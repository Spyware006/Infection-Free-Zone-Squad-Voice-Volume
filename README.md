<p align="center">
  <img src="images/banner.png" width="900" alt="Infection Free Zone - Squad Voice Volume banner">
</p>

# Infection Free Zone – Squad Voice Volume

*A lightweight BepInEx quality-of-life plugin.*

> **Reduce the noise, keep the important radio calls.**

![Version](https://img.shields.io/badge/version-1.0.0-brightgreen)
![BepInEx](https://img.shields.io/badge/BepInEx-5.x-blue)
![License](https://img.shields.io/badge/license-MIT-lightgrey)

## 📥 Download

➡️ **[Download the latest release](https://github.com/Spyware006/Infection-Free-Zone-Squad-Voice-Volume/releases/latest)**

---

## Features

- ✅ Reduces squad acknowledgement voice volume.
- ✅ Keeps important combat and alert radio transmissions unchanged.
- ✅ Lightweight Harmony patch.
- ✅ Configurable through a simple `.cfg` file.
- ✅ No game files are modified.

## Why?

Infection Free Zone features excellent radio voice acting. However, repeated squad acknowledgements such as “Roger” or “On my way” can become distracting during extended gameplay.

This mod lowers only those repetitive confirmations while preserving important combat and alert communications.

## Installation

1. Install **BepInEx 5 x64** for Infection Free Zone.
2. Launch the game once, then close it.
3. Copy **IFZ_SquadVoiceVolume.dll** into:

```
Infection Free Zone/
└── BepInEx/
    └── plugins/
```

4. Launch the game.

On first launch, the plugin automatically creates:

```
Infection Free Zone/
└── BepInEx/
    └── config/
        └── ifz.squadvoicevolume.cfg
```

## Configuration

Open:

```
Infection Free Zone/
└── BepInEx/
    └── config/
        └── ifz.squadvoicevolume.cfg
```

Default values:

```ini
[General]
Enabled = true

[Volume]
ChooseVolume = 0.10
GoVolume = 0.10
AttackVolume = 1.00
BuildingClearVolume = 1.00

[Debug]
DebugLog = false
```

| Setting | Description |
|---|---|
| `ChooseVolume` | Volume when selecting squads |
| `GoVolume` | Volume for movement confirmations |
| `AttackVolume` | Volume for attack acknowledgements |
| `BuildingClearVolume` | Volume for building clear confirmations |
| `DebugLog` | Keep this disabled unless troubleshooting |

`1.00` means original volume.  
`0.10` means 10% of the original volume.  
`0.00` mutes that category.

## Compatibility

| Item | Status |
|------|--------|
| Tested on | **2026-06-28** |
| Game branch | **Steam – Major Update #6** |
| BepInEx | **5.x** |
| Platform | **Windows** |

## Uninstall

Delete:

```
Infection Free Zone/
└── BepInEx/
    └── plugins/
        └── IFZ_SquadVoiceVolume.dll
```

Optionally delete the config file:

```
Infection Free Zone/
└── BepInEx/
    └── config/
        └── ifz.squadvoicevolume.cfg
```

## License

This project is released under the MIT License.

## Credits

Created by **Spyware006**.
