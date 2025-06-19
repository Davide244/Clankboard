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
