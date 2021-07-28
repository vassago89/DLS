#pragma once
#pragma warning(disable:4996)

extern "C" {
#include <libavformat/avformat.h> 
#include <libswscale/swscale.h>
#include <libavcodec/avcodec.h>
#include <libavutil/imgutils.h>
	namespace dlsunmanaged
	{
		struct FFmpegSt
		{
			int frameIndex;

			int streamIndex = -1;

			AVHWDeviceType deviceType;

			AVFormatContext *pFormatCtx = NULL;
			AVCodecContext *pCodecCtx = NULL;
			AVCodec *pCodex = NULL;

			AVFrame *pFrame = NULL;
			AVPacket *pPacket = NULL;

			SwsContext *pSwsCtx = NULL;
			AVFrame *pRgbFrame = NULL;
			//GPU
			AVBufferRef *hwDeviceCtx = NULL;
		};

		bool FFmpegOpen(const char* fileName, FFmpegSt *ffmpegSt, AVHWDeviceType type);
		bool FFmpegRead(FFmpegSt *ffmpegSt, unsigned char* buffer, int width, int height, int channels, unsigned char* devBuffer = NULL);
		void FFmpegRelease(FFmpegSt *ffmpegSt);
		bool FFmpegSeek(FFmpegSt *ffmpegSt, int index, double fps);
	}
}