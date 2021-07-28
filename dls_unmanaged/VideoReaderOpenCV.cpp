// dls_unmanaged.cpp : 정적 라이브러리를 위한 함수를 정의합니다.
//

#include "pch.h"
#include "VideoReaderOpenCV.hpp"
#include <opencv2/opencv.hpp>
#include <opencv2/core/cuda.hpp>

using namespace std;
using namespace cv;

namespace dlsunmanaged
{
	VideoReaderOpenCV::VideoReaderOpenCV(string path)
	{
		mCap = unique_ptr<VideoCapture>(new VideoCapture(path));
		mat = unique_ptr<Mat>(new Mat());
		
		mVideoInfo.FrameCount = (int)mCap->get(CAP_PROP_FRAME_COUNT);
		mVideoInfo.FramePos = (int)mCap->get(CAP_PROP_POS_FRAMES);

		mVideoInfo.FrameWidth = (int)mCap->get(CAP_PROP_FRAME_WIDTH);
		mVideoInfo.FrameHeight = (int)mCap->get(CAP_PROP_FRAME_HEIGHT);
		mVideoInfo.FPS = mCap->get(CAP_PROP_FPS);

		unsigned char* temp = NULL;
		Read(temp);
		Jump(1);

		mVideoInfo.Channels = mat->channels();
	}

	VideoReaderOpenCV::~VideoReaderOpenCV()
	{
		Release();
	}

	void VideoReaderOpenCV::Jump(int index)
	{
		mVideoInfo.FramePos = index;

		if (mVideoInfo.FramePos <= 1)
			mVideoInfo.FramePos = 1;

		if (mVideoInfo.FramePos >= mVideoInfo.FrameCount)
			mVideoInfo.FramePos = mVideoInfo.FrameCount;

		mCap->set(cv::CAP_PROP_POS_FRAMES, mVideoInfo.FramePos - 1);
	}

	bool VideoReaderOpenCV::Read(unsigned char*& buffer)
	{
		if (!mCap->isOpened())
			return false;

		mCap->read(*mat);
		if (buffer != NULL && mat->data)
			memcpy(buffer, mat->data, mVideoInfo.FrameWidth * mVideoInfo.FrameHeight * mat->channels());

		mVideoInfo.FramePos = mCap->get(CAP_PROP_POS_FRAMES);
	
		return true;
	}

	void VideoReaderOpenCV::Release()
	{
		if (mCap != nullptr)
		{
			mCap->release();
			mCap = nullptr;
		}
			
		if (mat != nullptr)
		{
			mat->release();
			mat = nullptr;
		}
	}
}