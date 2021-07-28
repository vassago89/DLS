#pragma once

#include "pch.h"
#include <cliext/map>
#include "../dls_unmanaged/framework.h"

using namespace System;
using namespace System::Collections::Generic;

typedef cliext::map<int, array<Byte>^> FrameMap;
namespace dlsmanaged {
	public ref class ImageManager
	{
	private:
		dlsunmanaged::IVideoReader* mVideoReader;
		FrameMap mFrameMap;
		//Dictionary<int, array<Byte>^> mDictionary;
	public:
		ImageManager();

		void Open(String^ string, bool isGpu);

		array<Byte>^ Read(int skipCount);

		void Jump(int index);

		bool IsDone()
		{
			if (mVideoReader == nullptr)
				return true;

			return mVideoReader->IsDone();
		}

		int GetWidth()
		{
			if (mVideoReader == nullptr)
				return -1;

			return (int)mVideoReader->GetWidth();
		}

		int GetHeight()
		{
			if (mVideoReader == nullptr)
				return -1;

			return (int)mVideoReader->GetHeight();
		}

		int GetChannels()
		{
			if (mVideoReader == nullptr)
				return -1;

			return (int)mVideoReader->GetChannels();
		}

		int GetCount()
		{
			if (mVideoReader == nullptr)
				return -1;

			return (int)mVideoReader->GetCount();
		}

		int GetPos()
		{
			if (mVideoReader == nullptr)
				return -1;

			return (int)mVideoReader->GetPos();
		}
	};
}
