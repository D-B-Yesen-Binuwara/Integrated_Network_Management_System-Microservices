import { useEffect, useMemo, useRef } from 'react'
import type { AlarmRecord, TopologyDevice, TopologyLink } from '../services/networkApi'

type DeviceMapProps = {
  devices: TopologyDevice[]
  links: TopologyLink[]
  alarms: AlarmRecord[]
}

/**
 * Leaflet map for topology devices. The map is intentionally imperative at
 * this boundary because Leaflet owns its DOM layers outside React's tree.
 */
export function DeviceMap({ devices, links, alarms }: DeviceMapProps) {
  const mapElement = useRef<HTMLDivElement>(null)
  const map = useRef<LeafletMap | null>(null)
  const layers = useRef<LeafletLayerGroup | null>(null)
  const runtimeError = typeof window !== 'undefined' && !window.L
  const activeAlarmCounts = useMemo(() => countActiveAlarms(alarms), [alarms])

  useEffect(() => {
    const leaflet = window.L
    if (!mapElement.current) return
    if (!leaflet) {
      return
    }

    // Use Leaflet's SVG renderer. It is more reliable for this marker/link
    // volume and avoids canvas redraw errors during React StrictMode remounts.
    const nextMap = leaflet.map(mapElement.current, { zoomControl: true, preferCanvas: false }).setView([0, 0], 2)
    leaflet.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '&copy; OpenStreetMap contributors',
    }).addTo(nextMap)

    map.current = nextMap
    layers.current = leaflet.layerGroup().addTo(nextMap)

    return () => {
      nextMap.remove()
      map.current = null
      layers.current = null
    }
  }, [])

  useEffect(() => {
    const leaflet = window.L
    const currentMap = map.current
    const currentLayers = layers.current
    if (!leaflet || !currentMap || !currentLayers) return

    currentLayers.clearLayers()
    const deviceById = new Map(devices.map((device) => [device.deviceId, device]))
    const positionedDevices = devices.filter(hasValidCoordinates)

    links.forEach((link) => {
      const parent = deviceById.get(link.parentDeviceId)
      const child = deviceById.get(link.childDeviceId)
      if (!parent || !child || !hasValidCoordinates(parent) || !hasValidCoordinates(child)) return

      leaflet.polyline(
        [[parent.latitude, parent.longitude], [child.latitude, child.longitude]],
        {
          color: link.linkStatus.toLowerCase() === 'up' ? '#83bdb3' : '#d6a56b',
          weight: 2,
          opacity: 0.72,
          dashArray: link.linkStatus.toLowerCase() === 'up' ? undefined : '6 7',
        },
      ).addTo(currentLayers)
    })

    positionedDevices.forEach((device) => {
      const alarmCount = activeAlarmCounts.get(device.deviceId) ?? 0
      const markerTone = getMarkerTone(device.status, alarmCount)
      const marker = leaflet.marker([device.latitude, device.longitude], {
        icon: leaflet.divIcon({
          className: 'leaflet-device-icon',
          html: `<span class="device-marker ${getDeviceTypeClass(device.deviceType)} ${markerTone}">${getDeviceGlyph(device.deviceType)}</span>`,
          iconSize: [38, 38],
          iconAnchor: [19, 19],
          popupAnchor: [0, -19],
        }),
      })

      marker.bindPopup(buildPopup(device, alarmCount)).addTo(currentLayers)
    })

    if (positionedDevices.length > 0) {
      currentMap.fitBounds(
        leaflet.latLngBounds(positionedDevices.map((device) => [device.latitude, device.longitude])),
        { padding: [28, 28], maxZoom: 13 },
      )
    } else {
      currentMap.setView([0, 0], 2)
    }

    window.setTimeout(() => currentMap.invalidateSize(), 0)
  }, [activeAlarmCounts, devices, links])

  return <div className="device-map" ref={mapElement} aria-label="Map showing network device locations">{runtimeError && <div className="map-runtime-error"><strong>Map renderer unavailable</strong><span>Leaflet could not be loaded. Check browser access to the pinned map runtime and refresh.</span></div>}</div>
}

function countActiveAlarms(alarms: AlarmRecord[]) {
  const counts = new Map<number, number>()
  alarms.filter((alarm) => alarm.isActive).forEach((alarm) => {
    counts.set(alarm.deviceId, (counts.get(alarm.deviceId) ?? 0) + 1)
  })
  return counts
}

function hasValidCoordinates(device: TopologyDevice) {
  return Number.isFinite(device.latitude) && Number.isFinite(device.longitude)
    && device.latitude >= -90 && device.latitude <= 90
    && device.longitude >= -180 && device.longitude <= 180
}

function getDeviceTypeClass(deviceType: string) {
  return `device-type-${deviceType.toLowerCase()}`
}

function getDeviceGlyph(deviceType: string) {
  const glyphs: Record<string, string> = {
    slbn: '⌁',
    cean: '◆',
    msan: '▦',
    customer: '●',
  }
  return glyphs[deviceType.toLowerCase()] ?? '•'
}

function getMarkerTone(status: string, alarmCount: number) {
  if (alarmCount > 0) return 'marker-alarm'
  const normalizedStatus = status.toLowerCase()
  if (normalizedStatus === 'down' || normalizedStatus === 'unreachable') return 'marker-down'
  if (normalizedStatus === 'impacted') return 'marker-impacted'
  return 'marker-up'
}

function buildPopup(device: TopologyDevice, alarmCount: number) {
  const alarmText = alarmCount > 0 ? `${alarmCount} active alarm${alarmCount === 1 ? '' : 's'}` : 'No active alarms'
  return `<div class="device-popup"><strong>${escapeHtml(device.deviceName)}</strong><span>${escapeHtml(device.deviceType)}</span><span>IP: ${escapeHtml(device.ip || 'Unavailable')}</span><span>Region / Province: ${escapeHtml(device.regionCode || 'Unavailable')} / ${escapeHtml(device.provinceCode || 'Unavailable')}</span><span>LEA: ${escapeHtml(device.leaCode || 'Unavailable')}</span><span>Priority: ${escapeHtml(device.priorityLevel)}</span><span>Status: ${escapeHtml(device.status)} · ${alarmText}</span></div>`
}

function escapeHtml(value: string) {
  return value.replace(/[&<>"']/g, (character) => ({
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#039;',
  })[character] ?? character)
}
