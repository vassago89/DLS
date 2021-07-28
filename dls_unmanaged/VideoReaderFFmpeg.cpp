// dls_unmanaged.cpp : 정적 라이브러리를 위한 함수를 정의합니다.
//

#include "pch.h"
#include "VideoReaderFFmpeg.hpp"
#include "../dls_unmanaged_cuda/Nv2ToRgbFunc.h"

#include <algorithm>

using namespace std;

namespace dlsunmanaged
{
	VideoReaderFFmpeg::VideoReaderFFmpeg(std::string path, bool isCuda) : bCuda(isCuda)
	{
		/*AV_HWDEVICE_TYPE_NONE,
		AV_HWDEVICE_TYPE_VDPAU,
		AV_HWDEVICE_TYPE_CUDA,
		AV_HWDEVICE_TYPE_VAAPI,
		AV_HWDEVICE_TYPE_DXVA2,
		AV_HWDEVICE_TYPE_QSV,
		AV_HWDEVICE_TYPE_VIDEOTOOLBOX,
		AV_HWDEVICE_TYPE_D3D11VA,
		AV_HWDEVICE_TYPE_DRM,
		AV_HWDEVICE_TYPE_OPENCL,
		AV_HWDEVICE_TYPE_MEDIACODEC,
		AV_HWDEVICE_TYPE_VULKAN*/

		if (ffmpegSt == nullptr)
			ffmpegSt = new FFmpegSt();

		if (bCuda)
		{
			if (FFmpegOpen(path.c_str(), ffmpegSt, AV_HWDEVICE_TYPE_CUDA) == false)
				Release();
		}
		else
		{
			if (FFmpegOpen(path.c_str(), ffmpegSt, AV_HWDEVICE_TYPE_NONE) == false)
				Release();
		}
		
		
		mVideoInfo.FrameWidth = ffmpegSt->pCodecCtx->width;
		mVideoInfo.FrameHeight = ffmpegSt->pCodecCtx->height;
		mVideoInfo.Channels = 3;//ffmpegSt.pCodecCtx->channels;
		mVideoInfo.FramePos = ffmpegSt->pFrame->coded_picture_number;
		mVideoInfo.FrameCount = ffmpegSt->pFormatCtx->streams[ffmpegSt->streamIndex]->nb_frames;
		mVideoInfo.FPS = av_q2d(ffmpegSt->pFormatCtx->streams[ffmpegSt->streamIndex]->r_frame_rate);

		if (bCuda)
		{
			cuda::CudaInit();
			cudaBuffer = cuda::CudaAlloc<unsigned char>((int)mVideoInfo.FrameWidth * 3, (int)mVideoInfo.FrameHeight);
			//unique_ptr<unsigned char> tmp(new unsigned char(mVideoInfo.FrameWidth * mVideoInfo.FrameHeight * mVideoInfo.Channels));
			//cudaBuffer = tmp.get();
		}
	}

	VideoReaderFFmpeg::~VideoReaderFFmpeg()
	{
		Release();
	}

	bool VideoReaderFFmpeg::Read(unsigned char*& buffer)
	{
		bool result = false;

		if (bCuda)
		{
			result = FFmpegRead(
					ffmpegSt,
					buffer,
					mVideoInfo.FrameWidth,
					mVideoInfo.FrameHeight,
					mVideoInfo.Channels,
					cudaBuffer);;
		}
		else
		{
			result = FFmpegRead(ffmpegSt, buffer, mVideoInfo.FrameWidth, mVideoInfo.FrameHeight, mVideoInfo.Channels);
		}

		if (result == true)
			mVideoInfo.FramePos = ffmpegSt->frameIndex;

		return result;
	}

	void VideoReaderFFmpeg::Jump(int index)
	{
		if (index < 0 || mVideoInfo.FrameCount <= index)
			return;

		FFmpegSeek(ffmpegSt, index, mVideoInfo.FPS);

		mVideoInfo.FramePos = ffmpegSt->frameIndex;
	}

	void VideoReaderFFmpeg::Release()
	{
		if (ffmpegSt != nullptr)
		{
			FFmpegRelease(ffmpegSt);
			delete ffmpegSt;
			ffmpegSt = nullptr;
		}
		
		if (cudaBuffer != nullptr)
		{
			cuda::CudaFree(cudaBuffer);
			cudaBuffer = nullptr;
		}
	}
}