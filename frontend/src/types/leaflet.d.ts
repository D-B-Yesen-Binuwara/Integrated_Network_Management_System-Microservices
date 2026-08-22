export {}

/** Minimal Leaflet surface used by DeviceMap. The runtime is loaded in index.html. */
declare global {
  type LeafletLatLngExpression = [number, number]

  type LeafletMap = {
    setView: (center: LeafletLatLngExpression, zoom: number) => LeafletMap
    fitBounds: (bounds: LeafletBounds, options?: { padding?: [number, number]; maxZoom?: number }) => LeafletMap
    invalidateSize: () => LeafletMap
    remove: () => void
  }

  type LeafletBounds = object
  type LeafletTarget = LeafletMap | LeafletLayerGroup

  type LeafletLayerGroup = {
    addTo: (map: LeafletMap) => LeafletLayerGroup
    clearLayers: () => LeafletLayerGroup
  }

  type LeafletTileLayer = {
    addTo: (map: LeafletMap) => LeafletTileLayer
  }

  type LeafletMarker = {
    addTo: (target: LeafletTarget) => LeafletMarker
    bindPopup: (content: string) => LeafletMarker
  }

  type LeafletPolyline = {
    addTo: (target: LeafletTarget) => LeafletPolyline
  }

  type LeafletIcon = object

  type LeafletNamespace = {
    map: (container: HTMLElement, options?: { zoomControl?: boolean; preferCanvas?: boolean }) => LeafletMap
    tileLayer: (url: string, options: { attribution: string }) => LeafletTileLayer
    layerGroup: () => LeafletLayerGroup
    marker: (position: LeafletLatLngExpression, options: { icon: LeafletIcon }) => LeafletMarker
    divIcon: (options: {
      className: string
      html: string
      iconSize: [number, number]
      iconAnchor: [number, number]
      popupAnchor: [number, number]
    }) => LeafletIcon
    polyline: (positions: LeafletLatLngExpression[], options: { color: string; weight: number; opacity: number; dashArray?: string }) => LeafletPolyline
    latLngBounds: (positions: LeafletLatLngExpression[]) => LeafletBounds
  }

  interface Window {
    L?: LeafletNamespace
  }
}
