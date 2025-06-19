/**
 * ==============================================================================
 *  File Name     : ClankAPO.h
 *  Author        : D244
 *  Editors       : N/A
 *  Purpose       : NMain header file for the ClankAPO project. Includes declaration of the CClankAPO class and necessary includes.
 *  Created On    : 2025-04-09
 *  License       : All rights reserved. (SUBJECT TO CHANGE)
 * ==============================================================================
 **/

#pragma once

// Audio engine includes
#include <audioenginebaseapo.h>
#include <audioengineextensionapo.h>
#include <baseaudioprocessingobject.h>

// WDK includes
#include <devicetopology.h>
#include <RTWorkQ.h>

// Generated headers
#include "ClankAPOdll.h"
#include "ClankAPOInterface.h"

#include "resource.h"
#include "vcpkg_installed/vcpkg/pkgs/wil_x64-windows/include/wil/com.h"

_Analysis_mode_(_Analysis_code_type_user_driver_) // Analysis mode for user-mode driver code


// Defines for ClankAPO
#define CLANKAPO_NAME L"ClankAPO"

#define PK_EQUAL(x, y)  ((x.fmtid == y.fmtid) && (x.pid == y.pid))
#define GUID_FORMAT_STRING "{%08x-%04x-%04x-%02x%02x-%02x%02x%02x%02x%02x%02x}"
#define GUID_FORMAT_ARGS(guidVal)  (guidVal).Data1, (guidVal).Data2, (guidVal).Data3, (guidVal).Data4[0], (guidVal).Data4[1], (guidVal).Data4[2], (guidVal).Data4[3], (guidVal).Data4[4], (guidVal).Data4[5], (guidVal).Data4[6], (guidVal).Data4[7]
#define NUM_OF_EFFECTS 1

// GUID for the ClankAPO MFX context (5db5b4c8-6c37-450e-93f5-1e275afdF87f)
DEFINE_GUID(CLANK_APO_MFX_CONTEXT, 0x5db5b4c8, 0x6c37, 0x450e, 0x93, 0xf5, 0x1e, 0x27, 0x5a, 0xfd, 0xf8, 0x7f);
// GUID for the ClankAPO SFX context (99817AE5-E6DC-4074-B513-8A872178DA12)
DEFINE_GUID(CLANK_APO_SFX_CONTEXT, 0x99817ae5, 0xe6dc, 0x4074, 0xb5, 0x13, 0x8a, 0x87, 0x21, 0x78, 0xda, 0x12);

LONG GetCurrentEffectsSetting(IPropertyStore* properties, PROPERTYKEY pkeyEnable, GUID processingMode);

// Definition of the CClankAPO class
class CClankAPO_MFX :
	public CComObjectRootEx<CComMultiThreadModel>,
	public CComCoClass<CClankAPO_MFX, &CLSID_CClankAPO_MFX>,
	public CBaseAudioProcessingObject,
	public IMMNotificationClient,
	public IAudioProcessingObjectNotifications,
	public IAudioSystemEffects3,
	public IClankAPO_MFX
{
public:
	CClankAPO_MFX()
		:CBaseAudioProcessingObject(CLANK_APO_MFX_CONTEXT),
		, m_hEffectsChangedEvent(NULL)
		, m_AudioProcessingMode(AUDIO_SIGNALPROCESSINGMODE_DEFAULT)
		, m_fEnableSwapMFX(FALSE)
	{
		m_pf32Coefficients = NULL;
	}

	virtual ~CSwapAPOMFX();    // destructor

	DECLARE_REGISTRY_RESOURCEID(IDR_CLANKAPO_MFX);

	BEGIN_COM_MAP(CClankAPO_MFX)
		COM_INTERFACE_ENTRY(IClankAPO_MFX)
		COM_INTERFACE_ENTRY(IAudioSystemEffects)
		COM_INTERFACE_ENTRY(IAudioSystemEffects2)
		COM_INTERFACE_ENTRY(IAudioSystemEffects3)
		// IAudioSystemEffectsCustomFormats may be optionally supported
		// by APOs that attach directly to the connector in the DEFAULT mode streaming graph
		COM_INTERFACE_ENTRY(IAudioSystemEffectsCustomFormats)
		COM_INTERFACE_ENTRY(IMMNotificationClient)
		COM_INTERFACE_ENTRY(IAudioProcessingObjectNotifications)
		COM_INTERFACE_ENTRY(IAudioProcessingObjectRT)
		COM_INTERFACE_ENTRY(IAudioProcessingObject)
		COM_INTERFACE_ENTRY(IAudioProcessingObjectConfiguration)
	END_COM_MAP()

	DECLARE_PROTECT_FINAL_CONSTRUCT();

public:
	STDMETHOD_(void, APOProcess)(UINT32 u32NumInputConnections,
		APO_CONNECTION_PROPERTY** ppInputConnections, UINT32 u32NumOutputConnections,
		APO_CONNECTION_PROPERTY** ppOutputConnections);

	STDMETHOD(GetLatency)(HNSTIME* pTime);

	STDMETHOD(LockForProcess)(UINT32 u32NumInputConnections,
		APO_CONNECTION_DESCRIPTOR** ppInputConnections,
		UINT32 u32NumOutputConnections, APO_CONNECTION_DESCRIPTOR** ppOutputConnections);

	STDMETHOD(Initialize)(UINT32 cbDataSize, BYTE* pbyData);

	// IAudioSystemEffects2
	STDMETHOD(GetEffectsList)(_Outptr_result_buffer_maybenull_(*pcEffects)  LPGUID* ppEffectsIds, _Out_ UINT* pcEffects, _In_ HANDLE Event);

	// IAudioSystemEffects3
	STDMETHOD(GetControllableSystemEffectsList)(_Outptr_result_buffer_maybenull_(*numEffects) AUDIO_SYSTEMEFFECT** effects, _Out_ UINT* numEffects, _In_opt_ HANDLE event);

	STDMETHOD(SetAudioSystemEffectState)(GUID effectId, AUDIO_SYSTEMEFFECT_STATE state);

	virtual HRESULT ValidateAndCacheConnectionInfo(
		UINT32 u32NumInputConnections,
		APO_CONNECTION_DESCRIPTOR** ppInputConnections,
		UINT32 u32NumOutputConnections,
		APO_CONNECTION_DESCRIPTOR** ppOutputConnections);

	// IMMNotificationClient
	STDMETHODIMP OnDeviceStateChanged(LPCWSTR pwstrDeviceId, DWORD dwNewState)
	{
		UNREFERENCED_PARAMETER(pwstrDeviceId);
		UNREFERENCED_PARAMETER(dwNewState);
		return S_OK;
	}
	STDMETHODIMP OnDeviceAdded(LPCWSTR pwstrDeviceId)
	{
		UNREFERENCED_PARAMETER(pwstrDeviceId);
		return S_OK;
	}
	STDMETHODIMP OnDeviceRemoved(LPCWSTR pwstrDeviceId)
	{
		UNREFERENCED_PARAMETER(pwstrDeviceId);
		return S_OK;
	}
	STDMETHODIMP OnDefaultDeviceChanged(EDataFlow flow, ERole role, LPCWSTR pwstrDefaultDeviceId)
	{
		UNREFERENCED_PARAMETER(flow);
		UNREFERENCED_PARAMETER(role);
		UNREFERENCED_PARAMETER(pwstrDefaultDeviceId);
		return S_OK;
	}
	STDMETHODIMP OnPropertyValueChanged(LPCWSTR pwstrDeviceId, const PROPERTYKEY key);

	// IAudioProcessingObjectNotifications
	STDMETHODIMP GetApoNotificationRegistrationInfo(_Out_writes_(*count) APO_NOTIFICATION_DESCRIPTOR** apoNotifications, _Out_ DWORD* count);
	STDMETHODIMP_(void) HandleNotification(_In_ APO_NOTIFICATION* apoNotification);

	// IAudioSystemEffectsCustomFormats
	// This interface may be optionally supported by APOs that attach directly to the connector in the DEFAULT mode streaming graph
	STDMETHODIMP GetFormatCount(UINT* pcFormats);
	STDMETHODIMP GetFormat(UINT nFormat, IAudioMediaType** ppFormat);
	STDMETHODIMP GetFormatRepresentation(UINT nFormat, _Outptr_ LPWSTR* ppwstrFormatRep);

	// IAudioProcessingObject
	STDMETHODIMP IsOutputFormatSupported(IAudioMediaType* pOppositeFormat, IAudioMediaType* pRequestedOutputFormat, IAudioMediaType** ppSupportedOutputFormat);

	STDMETHODIMP CheckCustomFormats(IAudioMediaType* pRequestedFormat);

	STDMETHODIMP DoWorkOnRealTimeThread();

	void HandleWorkItemCompleted(_In_ IRtwqAsyncResult* asyncResult);
};