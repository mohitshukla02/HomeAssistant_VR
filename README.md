# HomeAssistant_VR

Home Assistant control in a headset. A worldspace panel on Meta Quest that
drives real lights and devices over MQTT.

## What it does

The panel publishes to and subscribes from the Home Assistant MQTT bridge —
`msHA/Lights/Bedroom/...` for individual and grouped lights, `msHA/Devices/...`
for room devices. Subscribing as well as publishing matters: the panel reflects
what the house is actually doing, so it stays correct when something is changed
from a wall switch or a phone.

Layouts are designed in Figma and imported straight into Unity, and spatial
arrangement is prototyped in ShapesXR and imported alongside — so where the
panel sits relative to you is a design decision made in headset, not a
transform typed into the inspector.

## Scenes

| Scene | Purpose |
|---|---|
| `SampleScene` | The VR control panel |
| `2D_TemplateMQTTTest` | Flat-screen MQTT wiring, testable without a headset |

## Built with

Unity 2022.3.3f1 (URP) · Meta XR SDK 59 · smoothMQTT ·
Figma Converter for Unity · ShapesXR · Meta Quest Pro / Quest 3

## Related

[SmartHomeOS](https://github.com/mohitshukla02/SmartHomeOS) — the UI layer ·
[HA_Passthrough](https://github.com/mohitshukla02/HA_Passthrough) — the
passthrough build ·
[Test-SmoothMQTT](https://github.com/mohitshukla02/Test-SmoothMQTT) — the MQTT
groundwork

## Third-party assets

Bundles the Meta XR SDK, smoothMQTT, Figma Converter for Unity, ShapesXR,
TotalJSON, and DOTween, which remain under their own licenses.

## License

Copyright © 2026 Mohit Shukla. All rights reserved.

This repository is made publicly viewable for portfolio and demonstration
purposes only. No license is granted to use, copy, modify, merge, publish,
distribute, sublicense, or sell copies of HomeAssistant_VR or any part of
it, in whole or in part, without prior written permission from the
copyright holder.
