/**
 * ==============================================================================
 *  File Name     : ClankAPO.cpp
 *  Author        : D244
 *  Editors       : N/A
 *  Purpose       : Implentation file for the ClankAPO MFX APO class.
 *  Created On    : 2025-04-09
 *  License       : All rights reserved. (SUBJECT TO CHANGE)
 * ==============================================================================
 **/

#include <atlbase.h>
#include <atlcom.h>
#include <atlcoll.h>
#include <atlsync.h>
#include <mmreg.h>

#include "resource.h"
#include "ClankAPO.h"

CClankAPO_MFX::~CClankAPO_MFX()
{
	if (m_bRegisteredEndpointNotificationCallback)
	{
		m_spEnumerator->UnregisterEndpointNotificationCallback(this);
	}

	if (m_hEffectsChangedEvent != NULL)
	{
		CloseHandle(m_hEffectsChangedEvent);
	}

	if (NULL != m_pf32Coefficients)
	{
		AERT_Free(m_pf32Coefficients);
		m_pf32Coefficients = NULL;
	}
}

HRESULT CClankAPO_MFX::ValidateAndCacheConnectionInfo(UINT32 u32NumInputConnections, APO_CONNECTION_DESCRIPTOR** ppInputConnections, UINT32 u32NumOutputConnections, APO_CONNECTION_DESCRIPTOR** ppOutputConnections)
{
	ASSERT_NONREALTIME(); // This function should not be called in real-time processing.

	HRESULT hr = S_OK;
	CComPtr<IAudioMediaType> pInputFormat;
	UNCOMPRESSEDAUDIOFORMAT uncompressedAudioInputFormat, uncompressedAudioOutputFormat;
	FLOAT32 inverseChannelCount = 0.0f;

	UNREFERENCED_PARAMETER(u32NumOutputConnections);
	UNREFERENCED_PARAMETER(u32NumInputConnections);

	_ASSERTE(!m_bIsLocked);
	_ASSERTE(((0 == u32NumInputConnections) || (NULL != ppInputConnections)) &&
		((0 == u32NumOutputConnections) || (NULL != ppOutputConnections))); // Validate input and output connections

	EnterCriticalSection(&m_CritSec); // Thread safety: Lock the critical section

	hr = ppInputConnections[0]->pFormat->GetUncompressedAudioFormat(&uncompressedAudioInputFormat);
	if (FAILED(hr))
	{
		LeaveCriticalSection(&m_CritSec);
		return hr; // Return if unable to get input format
	}

	hr = ppOutputConnections[0]->pFormat->GetUncompressedAudioFormat(&uncompressedAudioOutputFormat);
	if (FAILED(hr))
	{
		LeaveCriticalSection(&m_CritSec);
		return hr; // Return if unable to get output format
	}

	_ASSERTE(uncompressedAudioOutputFormat.fFramesPerSecond == uncompressedAudioInputFormat.fFramesPerSecond);
	_ASSERTE(uncompressedAudioOutputFormat.dwSamplesPerFrame == uncompressedAudioInputFormat.dwSamplesPerFrame);

	LeaveCriticalSection(&m_CritSec); // Unlock the critical section
	return hr;
}
