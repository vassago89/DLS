#pragma once

#include "Nv2ToRgbFunc.h"

#ifndef max
#define max(a,b)  (((a) > (b)) ? (a) : (b))
#endif



#ifndef min
#define min(a,b)  (((a) < (b)) ? (a) : (b))
#endif

namespace dlsunmanaged {
	namespace cuda {
		__constant__ float constHueColorSpaceMat[9] = 
		{ 
			1.164f, 0.0f, 1.596f, 
			1.164f, -0.391f, -0.813f, 
			1.164f, 2.018f, 0.0f 
		};

		__global__ void NV12ToRGB(
			unsigned char* srcY, 
			unsigned char* srcUV,
			size_t nSourcePitch,
			unsigned char* dstImage, 
			size_t nDestPitch,
			unsigned int width,
			unsigned int height)
		{
			const int x = blockIdx.x * (blockDim.x << 1) + (threadIdx.x << 1);
			const int y = blockIdx.y * (blockDim.y << 1) + (threadIdx.y << 1);

			if (x >= width || y >= height)
				return;

			const int y_chroma = y >> 1;

			float y1 = srcY[y * nSourcePitch + x] - 16;
			float y2 = srcY[y * nSourcePitch + x + 1] - 16;
			float y3 = srcY[(y + 1) * nSourcePitch + x] - 16;
			float y4 = srcY[(y + 1) * nSourcePitch + x + 1] - 16;

			float cb  = srcUV[y_chroma * nSourcePitch + x] - 128;
			float cr = srcUV[y_chroma * nSourcePitch + x + 1] - 128;

			int index = y * nDestPitch + x * 3;
			dstImage[index++] =
				max(0, min(255, constHueColorSpaceMat[6] * y1
					+ constHueColorSpaceMat[7] * cb
					+ constHueColorSpaceMat[8] * cr));

			dstImage[index++] =
				max(0, min(255, constHueColorSpaceMat[3] * y1
					+ constHueColorSpaceMat[4] * cb
					+ constHueColorSpaceMat[5] * cr));

			dstImage[index++] =
				max(0, min(255, constHueColorSpaceMat[0] * y1
					+ constHueColorSpaceMat[1] * cb
					+ constHueColorSpaceMat[2] * cr));

			dstImage[index++] =
				max(0, min(255, constHueColorSpaceMat[6] * y2
					+ constHueColorSpaceMat[7] * cb
					+ constHueColorSpaceMat[8] * cr));

			dstImage[index++] =
				max(0, min(255, constHueColorSpaceMat[3] * y2
					+ constHueColorSpaceMat[4] * cb
					+ constHueColorSpaceMat[5] * cr));

			dstImage[index++] =
				max(0, min(255, constHueColorSpaceMat[0] * y2
					+ constHueColorSpaceMat[1] * cb
					+ constHueColorSpaceMat[2] * cr));

			index = (y + 1) * nDestPitch + x * 3;
			dstImage[index++] =
				max(0, min(255, constHueColorSpaceMat[6] * y3
					+ constHueColorSpaceMat[7] * cb
					+ constHueColorSpaceMat[8] * cr));

			dstImage[index++] =
				max(0, min(255, constHueColorSpaceMat[3] * y3
					+ constHueColorSpaceMat[4] * cb
					+ constHueColorSpaceMat[5] * cr));

			dstImage[index++] =
				max(0, min(255, constHueColorSpaceMat[0] * y3
					+ constHueColorSpaceMat[1] * cb
					+ constHueColorSpaceMat[2] * cr));

			dstImage[index++] =
				max(0, min(255, constHueColorSpaceMat[6] * y4
					+ constHueColorSpaceMat[7] * cb
					+ constHueColorSpaceMat[8] * cr));

			dstImage[index++] =
				max(0, min(255, constHueColorSpaceMat[3] * y4
					+ constHueColorSpaceMat[4] * cb
					+ constHueColorSpaceMat[5] * cr));

			dstImage[index++] =
				max(0, min(255, constHueColorSpaceMat[0] * y4
					+ constHueColorSpaceMat[1] * cb
					+ constHueColorSpaceMat[2] * cr));
		}

		void Nv2ToRgbFunc(
			unsigned char* srcY,
			unsigned char* srcUV,
			int srcPitch, 
			unsigned char* dest,
			int destPitch, 
			int width,
			int height)
		{
			// Final Stage: NV12toARGB color space conversion
			//checkCudaError(cudaDeviceSynchronize());

			/*unsigned char* src1 = cuda::CudaAlloc<unsigned char>(srcPitch, height);
			unsigned char* src2 = cuda::CudaAlloc<unsigned char>(srcPitch, height / 2);

			cuda::CudaCopyDevToDev(srcY, src1, srcPitch * height);
			cuda::CudaCopyDevToDev(srcUV, src2, srcPitch * height / 2);*/

			dim3 block(32, 8);
			dim3 grid((width + (2 * block.x) - 1) / (2 * block.x), (height + (2 * block.y) - 1) / (2 * block.y));

			NV12ToRGB << <grid, block, 0 >> > (
				srcY,
				srcUV,
				srcPitch,
				dest,
				destPitch,
				width,
				height);

			checkCudaError(cudaGetLastError());
			checkCudaError(cudaDeviceSynchronize());
		}

		void CudaInit()
		{
			cudaDeviceReset();

			int count = 0;
			checkCudaError(cudaGetDeviceCount(&count));

			int gpu = 0;
			checkCudaError(cudaSetDevice(gpu));
		}

		void CudaRelease()
		{
			cudaDeviceReset();
		}

		
	}
}