#include "pch.h"

#include "ImageManager.h"

using namespace Runtime::InteropServices;

namespace dlsmanaged {
	using namespace System;

	ImageManager::ImageManager()
	{

	}

	void ImageManager::Open(String^ path, bool isGpu)
	{
		if (mVideoReader != nullptr)
		{
			mVideoReader->Release();
			delete mVideoReader;
			mVideoReader = nullptr;
		}
		
		mFrameMap.clear();
		std::string str;
		MarshalString(path, &str);
		//mVideoReader = new dlsunmanaged::VideoReaderOpenCV(str);
		mVideoReader = new dlsunmanaged::VideoReaderFFmpeg(str, true);
	}

	array<Byte>^ ImageManager::Read(int skipCount)
	{
		if (mVideoReader == nullptr)
			return nullptr;

		auto index = MAX(1, GetPos() + skipCount);

		FrameMap::const_iterator founded = mFrameMap.find(index);

		if (founded != mFrameMap.end())
		{
			mVideoReader->SetPos(index);
			return founded.get_cref()->second;
		}

		auto len = mVideoReader->GetWidth() * mVideoReader->GetHeight() * mVideoReader->GetChannels();

		array<Byte> ^byteArray = nullptr;
		byteArray = gcnew array<Byte>(len);
		pin_ptr<Byte> ptr = &byteArray[0];
		Byte *buffer = (unsigned char *)ptr;

		if (skipCount != 1)
			mVideoReader->Jump(index);
		
		auto pos = GetPos();
		while (mFrameMap.find(index) == mFrameMap.end())
		{
			if (mFrameMap.find(pos) != mFrameMap.end())
			{
				pos++;
				continue;
			}

			if (mVideoReader->Read(buffer) == false)
				return nullptr;

			mFrameMap.insert(FrameMap::make_value(pos, byteArray));
			pos = mVideoReader->GetPos();
		};

		return mFrameMap.find(index).get_cref()->second;
	}

	void ImageManager::Jump(int index)
	{
		mVideoReader->Jump(index);
	}
}
