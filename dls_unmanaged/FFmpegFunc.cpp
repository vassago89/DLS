#include "pch.h"
#include "FFmpegFunc.h"
#include "../dls_unmanaged_cuda/Nv2ToRgbFunc.h"

namespace dlsunmanaged
{
	extern "C" {

		bool FFmpegOpen(const char* fileName, FFmpegSt *ffmpegSt, AVHWDeviceType type)
		{
			if (avformat_open_input(&ffmpegSt->pFormatCtx, fileName, NULL, NULL) < 0)
				return false;

			if (avformat_find_stream_info(ffmpegSt->pFormatCtx, NULL) < 0)
				return false;

			ffmpegSt->streamIndex = av_find_best_stream(ffmpegSt->pFormatCtx, AVMEDIA_TYPE_VIDEO, -1, -1, &ffmpegSt->pCodex, 0);
			ffmpegSt->pCodecCtx = avcodec_alloc_context3(ffmpegSt->pCodex);

			if (avcodec_parameters_to_context(ffmpegSt->pCodecCtx, ffmpegSt->pFormatCtx->streams[ffmpegSt->streamIndex]->codecpar) < 0)
				return false;

			ffmpegSt->deviceType = type;

			ffmpegSt->pFrame = av_frame_alloc();
			ffmpegSt->pPacket = av_packet_alloc();

			if (type == AV_HWDEVICE_TYPE_CUDA)
			{
				if (av_hwdevice_ctx_create(&ffmpegSt->hwDeviceCtx, type, NULL, NULL, 0) < 0)
					return false;

				ffmpegSt->pCodecCtx->hw_device_ctx = av_buffer_ref(ffmpegSt->hwDeviceCtx);
				//ffmpegSt->pCodecCtx->thread_count = 4;
				//ffmpegSt->pCodecCtx->thread_type = FF_THREAD_FRAME;
			}

			if (avcodec_open2(ffmpegSt->pCodecCtx, ffmpegSt->pCodex, NULL) < 0)
				return false;

			if (type != AV_HWDEVICE_TYPE_CUDA)
			{				
				ffmpegSt->pRgbFrame = av_frame_alloc();

				auto numBytes = avpicture_get_size(AV_PIX_FMT_BGR24, ffmpegSt->pCodecCtx->width, ffmpegSt->pCodecCtx->height);
				unsigned char* buffer = (unsigned char*)malloc(numBytes * sizeof(unsigned char));

				avpicture_fill((AVPicture *)ffmpegSt->pRgbFrame, buffer, AV_PIX_FMT_BGR24, ffmpegSt->pCodecCtx->width, ffmpegSt->pCodecCtx->height);
			}

			return true;
		}

		bool FFmpegRead(FFmpegSt *ffmpegSt, unsigned char* buffer, int width, int height, int channels, unsigned char* devBuffer)
		{
			int gotFrame = 0;

			if (av_read_frame(ffmpegSt->pFormatCtx, ffmpegSt->pPacket)
				//|| avcodec_decode_video2(ffmpegSt->pCodecCtx, ffmpegSt->pFrame, &gotFrame, ffmpegSt->pPacket) < 0
				|| avcodec_send_packet(ffmpegSt->pCodecCtx, ffmpegSt->pPacket)
				|| avcodec_receive_frame(ffmpegSt->pCodecCtx, ffmpegSt->pFrame)
				|| buffer == nullptr)
			{
				av_packet_unref(ffmpegSt->pPacket);
				return false;
			}

			ffmpegSt->frameIndex = ffmpegSt->pPacket->dts / ffmpegSt->pPacket->duration + 1;
			av_packet_unref(ffmpegSt->pPacket);

			if (ffmpegSt->deviceType == AV_HWDEVICE_TYPE_CUDA)
			{
				cuda::Nv2ToRgbFunc(
					ffmpegSt->pFrame->data[0],
					ffmpegSt->pFrame->data[1],
					ffmpegSt->pFrame->linesize[0],
					devBuffer,
					width * channels,
					width,
					height);

				cuda::CudaCopyToMemory(devBuffer, buffer, width * height * channels * sizeof(unsigned char));
			}
			else
			{
				AVFrame *pFrame = ffmpegSt->pFrame;
				if (ffmpegSt->pSwsCtx == NULL)
				{
					ffmpegSt->pSwsCtx = sws_getContext(
						pFrame->width,
						pFrame->height,
						(AVPixelFormat)pFrame->format,
						pFrame->width,
						pFrame->height,
						AV_PIX_FMT_BGR24,
						SWS_FAST_BILINEAR, NULL, NULL, NULL);

					if (ffmpegSt->pSwsCtx == NULL)
						return false;
				}

				int output_Height = sws_scale(
					ffmpegSt->pSwsCtx,
					pFrame->data,
					pFrame->linesize,
					0,
					ffmpegSt->pCodecCtx->height,
					ffmpegSt->pRgbFrame->data,
					ffmpegSt->pRgbFrame->linesize);

				memcpy(buffer, ffmpegSt->pRgbFrame->data[0], (int)(pFrame->width * pFrame->height * 3));
			}

			return true;
 		}

		void FFmpegRelease(FFmpegSt *ffmpegSt)
		{
			if (ffmpegSt->pFormatCtx != NULL)
			{
				avformat_close_input(&ffmpegSt->pFormatCtx);
				ffmpegSt->pFormatCtx = NULL;
			}
			
			if (ffmpegSt->pCodecCtx != NULL)
			{
				avcodec_free_context(&ffmpegSt->pCodecCtx);
				ffmpegSt->pCodecCtx = NULL;
			}

			if (ffmpegSt->pFrame != NULL)
			{
				av_frame_free(&ffmpegSt->pFrame);
				ffmpegSt->pFrame = NULL;
			}


			if (ffmpegSt->pRgbFrame != NULL)
			{
				av_frame_free(&ffmpegSt->pRgbFrame);
				ffmpegSt->pRgbFrame = NULL;
			}
			
			if (ffmpegSt->pPacket != NULL)
			{
				av_packet_free(&ffmpegSt->pPacket);
				ffmpegSt->pPacket = NULL;
			}

			if (ffmpegSt->hwDeviceCtx != NULL)
			{
				av_buffer_unref(&ffmpegSt->hwDeviceCtx);
				ffmpegSt->hwDeviceCtx = NULL;
			}

			if (ffmpegSt->pSwsCtx != NULL)
			{
				sws_freeContext(ffmpegSt->pSwsCtx);
				ffmpegSt->pSwsCtx = NULL;
			}
		}

		bool FFmpegSeek(FFmpegSt *ffmpegSt, int index, double fps)
		{
			auto timeStamp = (index -1) * (int64_t(ffmpegSt->pCodecCtx->time_base.num) * AV_TIME_BASE) / int64_t(ffmpegSt->pCodecCtx->time_base.den);
			
			if (av_seek_frame(ffmpegSt->pFormatCtx, -1, timeStamp, AVSEEK_FLAG_BACKWARD) >= 0)
			{
				avcodec_flush_buffers(ffmpegSt->pCodecCtx);

				av_read_frame(ffmpegSt->pFormatCtx, ffmpegSt->pPacket);
				avcodec_send_packet(ffmpegSt->pCodecCtx, ffmpegSt->pPacket);
				avcodec_receive_frame(ffmpegSt->pCodecCtx, ffmpegSt->pFrame);
				ffmpegSt->frameIndex = ffmpegSt->pPacket->dts / ffmpegSt->pPacket->duration + 1;
				av_packet_unref(ffmpegSt->pPacket);

				if (index == ffmpegSt->frameIndex)
				{
					av_seek_frame(ffmpegSt->pFormatCtx, -1, timeStamp, AVSEEK_FLAG_BACKWARD);
					avcodec_flush_buffers(ffmpegSt->pCodecCtx);
				}
				/*else
				{
					while (index - 1 > ffmpegSt->frameIndex)
					{
						av_read_frame(ffmpegSt->pFormatCtx, ffmpegSt->pPacket);
						avcodec_send_packet(ffmpegSt->pCodecCtx, ffmpegSt->pPacket);
						avcodec_receive_frame(ffmpegSt->pCodecCtx, ffmpegSt->pFrame);

						ffmpegSt->frameIndex = ffmpegSt->pPacket->dts / ffmpegSt->pPacket->duration + 1;
						av_packet_unref(ffmpegSt->pPacket);
					}
				}*/

				return true;
			}
			
			return false;
		}
	}
}