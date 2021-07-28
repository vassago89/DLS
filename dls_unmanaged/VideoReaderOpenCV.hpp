#pragma once
#include "pch.h"
#include <opencv2/opencv.hpp>
#include "IVideoReader.hpp"

namespace dlsunmanaged {

	class VideoReaderOpenCV : public IVideoReader
	{

		// TODO: 여기에 이 클래스에 대한 메서드를 추가합니다.
	private:
		std::unique_ptr<cv::VideoCapture> mCap;
		std::unique_ptr<cv::Mat> mat;
		VideoInfo mVideoInfo;
	public:
		VideoReaderOpenCV(std::string path);
		~VideoReaderOpenCV();

		bool Read(unsigned char*& buffer);
		void Jump(int index);
		void Release();

		int GetCount()
		{
			return mVideoInfo.FrameCount;
		}

		int GetPos()
		{
			return mVideoInfo.FramePos;
		}

		bool IsDone()
		{
			return GetCount() < GetPos() + 1;
		}

		int GetWidth()
		{
			return mVideoInfo.FrameWidth;
		}

		int GetHeight()
		{
			return mVideoInfo.FrameHeight;
		}

		int GetChannels()
		{
			return mat->channels();
		}

		void SetPos(int pos)
		{
			mVideoInfo.FramePos = pos;
		}
	};
}