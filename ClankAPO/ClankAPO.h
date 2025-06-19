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

};