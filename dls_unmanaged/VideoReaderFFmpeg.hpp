#pragma once
#include "pch.h"
#include "FFmpegFunc.h"
#include <string>
#include "IVideoReader.hpp"

namespace dlsunmanaged {
	class VideoReaderFFmpeg : public IVideoReader
	{

		// TODO: 여기에 이 클래스에 대한 메서드를 추가합니다.
	private:
		VideoInfo mVideoInfo;
		FFmpegSt* ffmpegSt = nullptr;
		bool bCuda;
		unsigned char* cudaBuffer = nullptr;
	public:
		VideoReaderFFmpeg(std::string path, bool isCuda);
		~VideoReaderFFmpeg();

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
			return GetCount() == GetPos();
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
			return mVideoInfo.Channels;
		}

		void SetPos(int pos)
		{
			mVideoInfo.FramePos = pos;
		}
	};
}