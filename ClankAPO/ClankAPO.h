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

class ClankMFXApoAsyncCallback :
	public IRtwqAsyncCallback
{
private:
	DWORD m_queueId;
	volatile ULONG _refCount = 1;

public:
	ClankMFXApoAsyncCallback(DWORD queueId) : m_queueId(queueId)
	{
	}

	static HRESULT Create(_Outptr_ ClankMFXApoAsyncCallback** workItemOut, DWORD queueId);

	// IRtwqAsyncCallback
	STDMETHOD(GetParameters)(_Out_opt_ DWORD* pdwFlags, _Out_opt_ DWORD* pdwQueue)
	{
		*pdwFlags = 0;
		*pdwQueue = m_queueId;
		return S_OK;
	}
	STDMETHOD(Invoke)(_In_ IRtwqAsyncResult* asyncResult);

	// IUnknown (needed for IRtwqAsyncCallback)
	STDMETHOD(QueryInterface)(REFIID riid, __deref_out void** interfaceOut);
	STDMETHOD_(ULONG, AddRef)();
	STDMETHOD_(ULONG, Release)();
};

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
    /// <summary>
    /// Constructor. Initializes the APO with static registration properties.
    /// </summary>
    CClankAPO_MFX()  
        : CBaseAudioProcessingObject(&sm_RegProperties)  
        , m_hEffectsChangedEvent(NULL)  
        , m_AudioProcessingMode(AUDIO_SIGNALPROCESSINGMODE_DEFAULT)  
        , m_fEnableSwapMFX(FALSE)  
    {  
        m_pf32Coefficients = NULL;  
    }  

    /// <summary>
    /// Static registration properties for the APO.
    /// </summary>
    const APO_REG_PROPERTIES CClankAPO_MFX::sm_RegProperties = {  
        // Initialize with appropriate values for your APO  
        CLSID_CClankAPO_MFX,  
        DEFAULT_APOREG_FLAGS,  
        L"ClankAPO MFX",  
        L"Copyright (C) 2025 D244 (Davide_24)",  
        1, // Major version  
        0, // Minor version  
        DEFAULT_APOREG_MININPUTCONNECTIONS,  
        DEFAULT_APOREG_MAXINPUTCONNECTIONS,  
        DEFAULT_APOREG_MINOUTPUTCONNECTIONS,  
        DEFAULT_APOREG_MAXOUTPUTCONNECTIONS,  
        DEFAULT_APOREG_MAXINSTANCES,  
        1, // Number of APO interfaces  
        __uuidof(IAudioProcessingObject)  
    };

    /// <summary>
    /// Destructor.
    /// </summary>
	virtual ~CClankAPO_MFX();    // destructor

	DECLARE_REGISTRY_RESOURCEID(IDR_CLANKAPO_MFX);

	BEGIN_COM_MAP(CClankAPO_MFX)
		COM_INTERFACE_ENTRY(IClankAPO_MFX)
		COM_INTERFACE_ENTRY(IAudioSystemEffects)
		COM_INTERFACE_ENTRY(IAudioSystemEffects2)
		COM_INTERFACE_ENTRY(IAudioSystemEffects3)
		// IAudioSystemEffectsCustomFormats may be optionally supported
		// by APOs that attach directly to the connector in the DEFAULT mode streaming graph
		//COM_INTERFACE_ENTRY(IAudioSystemEffectsCustomFormats)
		COM_INTERFACE_ENTRY(IMMNotificationClient)
		COM_INTERFACE_ENTRY(IAudioProcessingObjectNotifications)
		COM_INTERFACE_ENTRY(IAudioProcessingObjectRT)
		COM_INTERFACE_ENTRY(IAudioProcessingObject)
		COM_INTERFACE_ENTRY(IAudioProcessingObjectConfiguration)
	END_COM_MAP()

	DECLARE_PROTECT_FINAL_CONSTRUCT();

public:
    /// <summary>
    /// Processes audio data.
    /// </summary>
	STDMETHOD_(void, APOProcess)(UINT32 u32NumInputConnections,
		APO_CONNECTION_PROPERTY** ppInputConnections, UINT32 u32NumOutputConnections,
		APO_CONNECTION_PROPERTY** ppOutputConnections);

    /// <summary>
    /// Gets the latency of the APO.
    /// </summary>
	STDMETHOD(GetLatency)(HNSTIME* pTime);

    /// <summary>
    /// Locks the APO for processing.
    /// </summary>
	STDMETHOD(LockForProcess)(UINT32 u32NumInputConnections,
		APO_CONNECTION_DESCRIPTOR** ppInputConnections,
		UINT32 u32NumOutputConnections, APO_CONNECTION_DESCRIPTOR** ppOutputConnections);

    /// <summary>
    /// Initializes the APO.
    /// </summary>
	STDMETHOD(Initialize)(UINT32 cbDataSize, BYTE* pbyData);

	// IAudioSystemEffects2
    /// <summary>
    /// Gets the list of effects.
    /// </summary>
	STDMETHOD(GetEffectsList)(_Outptr_result_buffer_maybenull_(*pcEffects)  LPGUID* ppEffectsIds, _Out_ UINT* pcEffects, _In_ HANDLE Event);

	// IAudioSystemEffects3
    /// <summary>
    /// Gets the list of controllable system effects.
    /// </summary>
	STDMETHOD(GetControllableSystemEffectsList)(_Outptr_result_buffer_maybenull_(*numEffects) AUDIO_SYSTEMEFFECT** effects, _Out_ UINT* numEffects, _In_opt_ HANDLE event);

    /// <summary>
    /// Sets the state of a system effect.
    /// </summary>
	STDMETHOD(SetAudioSystemEffectState)(GUID effectId, AUDIO_SYSTEMEFFECT_STATE state);

    /// <summary>
    /// Validates and caches connection information.
    /// </summary>
	virtual HRESULT ValidateAndCacheConnectionInfo(
		UINT32 u32NumInputConnections,
		APO_CONNECTION_DESCRIPTOR** ppInputConnections,
		UINT32 u32NumOutputConnections,
		APO_CONNECTION_DESCRIPTOR** ppOutputConnections);

	// IMMNotificationClient
    /// <summary>
    /// Called when the device state changes.
    /// </summary>
	STDMETHODIMP OnDeviceStateChanged(LPCWSTR pwstrDeviceId, DWORD dwNewState)
	{
		UNREFERENCED_PARAMETER(pwstrDeviceId);
		UNREFERENCED_PARAMETER(dwNewState);
		return S_OK;
	}
    /// <summary>
    /// Called when a device is added.
    /// </summary>
	STDMETHODIMP OnDeviceAdded(LPCWSTR pwstrDeviceId)
	{
		UNREFERENCED_PARAMETER(pwstrDeviceId);
		return S_OK;
	}
    /// <summary>
    /// Called when a device is removed.
    /// </summary>
	STDMETHODIMP OnDeviceRemoved(LPCWSTR pwstrDeviceId)
	{
		UNREFERENCED_PARAMETER(pwstrDeviceId);
		return S_OK;
	}
    /// <summary>
    /// Called when the default device changes.
    /// </summary>
	STDMETHODIMP OnDefaultDeviceChanged(EDataFlow flow, ERole role, LPCWSTR pwstrDefaultDeviceId)
	{
		UNREFERENCED_PARAMETER(flow);
		UNREFERENCED_PARAMETER(role);
		UNREFERENCED_PARAMETER(pwstrDefaultDeviceId);
		return S_OK;
	}
    /// <summary>
    /// Called when a property value changes.
    /// </summary>
	STDMETHODIMP OnPropertyValueChanged(LPCWSTR pwstrDeviceId, const PROPERTYKEY key);

	// IAudioProcessingObjectNotifications
    /// <summary>
    /// Gets APO notification registration info.
    /// </summary>
	STDMETHODIMP GetApoNotificationRegistrationInfo(_Out_writes_(*count) APO_NOTIFICATION_DESCRIPTOR** apoNotifications, _Out_ DWORD* count);
    /// <summary>
    /// Handles an APO notification.
    /// </summary>
	STDMETHODIMP_(void) HandleNotification(_In_ APO_NOTIFICATION* apoNotification);

	// IAudioSystemEffectsCustomFormats
	// This interface may be optionally supported by APOs that attach directly to the connector in the DEFAULT mode streaming graph
    /// <summary>
    /// Gets the number of supported formats.
    /// </summary>
	STDMETHODIMP GetFormatCount(UINT* pcFormats);
    /// <summary>
    /// Gets a supported format.
    /// </summary>
	STDMETHODIMP GetFormat(UINT nFormat, IAudioMediaType** ppFormat);
    /// <summary>
    /// Gets a string representation of a format.
    /// </summary>
	STDMETHODIMP GetFormatRepresentation(UINT nFormat, _Outptr_ LPWSTR* ppwstrFormatRep);

	// IAudioProcessingObject
    /// <summary>
    /// Checks if the output format is supported.
    /// </summary>
	STDMETHODIMP IsOutputFormatSupported(IAudioMediaType* pOppositeFormat, IAudioMediaType* pRequestedOutputFormat, IAudioMediaType** ppSupportedOutputFormat);

    /// <summary>
    /// Checks custom formats.
    /// </summary>
	STDMETHODIMP CheckCustomFormats(IAudioMediaType* pRequestedFormat);

    /// <summary>
    /// Performs work on the real-time thread.
    /// </summary>
	STDMETHODIMP DoWorkOnRealTimeThread();

    /// <summary>
    /// Handles completion of a work item.
    /// </summary>
	void HandleWorkItemCompleted(_In_ IRtwqAsyncResult* asyncResult);

	public:
        /// <summary>
        /// Indicates if the MFX swap effect is enabled.
        /// </summary>
		LONG                                    m_fEnableSwapMFX;
        /// <summary>
        /// The current audio processing mode.
        /// </summary>
		GUID                                    m_AudioProcessingMode;
        /// <summary>
        /// The associated audio device.
        /// </summary>
		wil::com_ptr_nothrow<IMMDevice>         m_device;
        /// <summary>
        /// Property store for APO system effects.
        /// </summary>
		CComPtr<IPropertyStore>                 m_spAPOSystemEffectsProperties;
        /// <summary>
        /// Device enumerator.
        /// </summary>
		CComPtr<IMMDeviceEnumerator>            m_spEnumerator;
        /// <summary>
        /// Information about the effects.
        /// </summary>
		AUDIO_SYSTEMEFFECT                      m_effectInfos[NUM_OF_EFFECTS];

		// Locked memory
        /// <summary>
        /// Pointer to filter coefficients.
        /// </summary>
		FLOAT32* m_pf32Coefficients;

	private:
        /// <summary>
        /// Critical section for effect state changes.
        /// </summary>
		CCriticalSection                        m_EffectsLock;
        /// <summary>
        /// Event handle for effects changed.
        /// </summary>
		HANDLE                                  m_hEffectsChangedEvent;
        /// <summary>
        /// Indicates if endpoint notification callback is registered.
        /// </summary>
		BOOL m_bRegisteredEndpointNotificationCallback = FALSE;

        /// <summary>
        /// User property store.
        /// </summary>
		wil::com_ptr_nothrow<IPropertyStore> m_userStore;
        /// <summary>
        /// Logging service for the APO.
        /// </summary>
		wil::com_ptr_nothrow<IAudioProcessingObjectLoggingService> m_apoLoggingService;

        /// <summary>
        /// Work queue ID.
        /// </summary>
		DWORD m_queueId = 0;
        /// <summary>
        /// Asynchronous callback for MFX APO.
        /// </summary>
		wil::com_ptr_nothrow<ClankMFXApoAsyncCallback> m_asyncCallback;

        /// <summary>
        /// Proprietary communication with the driver.
        /// </summary>
		HRESULT ProprietaryCommunicationWithDriver(IMMDeviceCollection* pDeviceCollection, UINT nSoftwareIoDeviceInCollection, UINT nSoftwareIoConnectorIndex);
};