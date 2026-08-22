import { useMemo, useState, type FormEvent } from 'react'
import { Icon } from '../components/Icon'
import { PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiRequest } from '../hooks/useApiRequest'
import { createDevice, getEmployees, getLeas, getProvinces, getRegions, getTopologyDevices, type EmployeeRecord, type LeaRecord, type ProvinceRecord, type RegionRecord } from '../services/networkApi'

type DeviceForm = {
  deviceName: string
  deviceType: string
  ip: string
  priorityLevel: string
  latitude: string
  longitude: string
  regionCode: string
  provinceCode: string
  leaCode: string
  assignedEngineerId: string
}

const initialForm: DeviceForm = {
  deviceName: '', deviceType: 'SLBN', ip: '', priorityLevel: 'Low', latitude: '', longitude: '',
  regionCode: '', provinceCode: '', leaCode: '', assignedEngineerId: '',
}

type DeviceFormMetadata = { regions: RegionRecord[]; provinces: ProvinceRecord[]; leas: LeaRecord[]; employees: EmployeeRecord[] }

export function InventoryPage() {
  const [query, setQuery] = useState('')
  const [deviceType, setDeviceType] = useState('all')
  const [isFormOpen, setIsFormOpen] = useState(false)
  const [form, setForm] = useState<DeviceForm>(initialForm)
  const [formError, setFormError] = useState<string | null>(null)
  const resource = useApiRequest('topology-devices', getTopologyDevices)
  const metadata = useApiRequest('device-form-metadata', async (signal): Promise<DeviceFormMetadata> => {
    const [regions, provinces, leas, employees] = await Promise.all([getRegions(signal), getProvinces(signal), getLeas(signal), getEmployees(signal)])
    return { regions, provinces, leas, employees }
  })
  // The form deliberately cascades from region to province to LEA. The
  // topology service remains the source of truth and derives parent codes.
  const devices = useMemo(() => (resource.data ?? []).filter((device) => {
    const matchesType = deviceType === 'all' || device.deviceType === deviceType
    const matchesQuery = JSON.stringify(device).toLowerCase().includes(query.toLowerCase())
    return matchesType && matchesQuery
  }), [deviceType, query, resource.data])
  const provinces = useMemo(() => (metadata.data?.provinces ?? []).filter((province) => !form.regionCode || province.region?.regionCode === form.regionCode), [form.regionCode, metadata.data?.provinces])
  const leas = useMemo(() => (metadata.data?.leas ?? []).filter((lea) => !form.provinceCode || lea.province?.provinceCode === form.provinceCode), [form.provinceCode, metadata.data?.leas])
  const engineers = useMemo(() => (metadata.data?.employees ?? []).filter((employee) => employee.leaCode === form.leaCode && (employee.roleName ?? '').toLowerCase().includes('lea')), [form.leaCode, metadata.data?.employees])

  const updateForm = (field: keyof DeviceForm, value: string) => {
    setForm((current) => ({ ...current, [field]: value }))
    setFormError(null)
  }

  const handleRegionChange = (value: string) => setForm((current) => ({ ...current, regionCode: value, provinceCode: '', leaCode: '', assignedEngineerId: '' }))
  const handleProvinceChange = (value: string) => setForm((current) => ({ ...current, provinceCode: value, leaCode: '', assignedEngineerId: '' }))
  const handleLeaChange = (value: string) => setForm((current) => ({ ...current, leaCode: value, assignedEngineerId: '' }))

  const submitDevice = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    if (!form.deviceName.trim() || !form.leaCode || !form.assignedEngineerId || !form.latitude || !form.longitude) {
      setFormError('Device name, location, LEA, and an assigned LEA engineer are required.')
      return
    }
    setFormError(null)
    try {
      await createDevice({
        deviceName: form.deviceName.trim(), deviceType: form.deviceType, ip: form.ip.trim(), priorityLevel: form.priorityLevel,
        latitude: Number(form.latitude), longitude: Number(form.longitude), leaCode: form.leaCode, assignedEngineerId: Number(form.assignedEngineerId),
      })
      setForm(initialForm)
      setIsFormOpen(false)
      resource.reload()
    } catch (reason: unknown) {
      setFormError(reason instanceof Error ? reason.message : 'The device could not be created.')
    }
  }

  return <>
    <PageHeader eyebrow="Network intelligence / Inventory" title="Device inventory" subtitle="One place for the devices that make up the service topology.">
      <button className="secondary-button" type="button" onClick={resource.reload}><Icon name="refresh" />Refresh</button>
      <button className="primary-button" type="button" onClick={() => { setIsFormOpen(true); setFormError(null) }}><Icon name="device" />Add device</button>
    </PageHeader>
    <div className="filter-bar"><label className="search-field"><Icon name="search" /><input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search devices, codes, vendors" aria-label="Search devices" /></label><label className="select-field"><select aria-label="Filter device type" value={deviceType} onChange={(event) => setDeviceType(event.target.value)}><option value="all">All device types</option><option value="SLBN">SLBN</option><option value="CEAN">CEAN</option><option value="MSAN">MSAN</option><option value="Customer">Customer</option></select></label></div>
    {isFormOpen && <Panel title="Register device" description="Select the LEA first. Province and region are derived from the selected LEA by the topology service." className="device-form-panel">
      {formError && <div className="state-error management-error"><strong>Device was not saved</strong><span>{formError}</span></div>}
      <form className="device-form" onSubmit={(event) => void submitDevice(event)}>
        <label>Device name<input required value={form.deviceName} onChange={(event) => updateForm('deviceName', event.target.value)} placeholder="e.g. Colombo core node" /></label>
        <label>Device type<select value={form.deviceType} onChange={(event) => updateForm('deviceType', event.target.value)}><option>SLBN</option><option>CEAN</option><option>MSAN</option><option>Customer</option></select></label>
        <label>IP address<input value={form.ip} onChange={(event) => updateForm('ip', event.target.value)} placeholder="10.0.0.1" /></label>
        <label>Priority level<select value={form.priorityLevel} onChange={(event) => updateForm('priorityLevel', event.target.value)}><option>Low</option><option>Avg</option><option>High</option><option>Critical</option></select></label>
        <label>Region<select value={form.regionCode} onChange={(event) => handleRegionChange(event.target.value)}><option value="">Select region</option>{(metadata.data?.regions ?? []).map((region) => <option key={region.regionId} value={region.regionCode}>{region.regionCode} · {region.name}</option>)}</select></label>
        <label>Province<select value={form.provinceCode} onChange={(event) => handleProvinceChange(event.target.value)} disabled={!form.regionCode}><option value="">Select province</option>{provinces.map((province) => <option key={province.provinceId} value={province.provinceCode}>{province.provinceCode} · {province.name}</option>)}</select></label>
        <label>LEA<select required value={form.leaCode} onChange={(event) => handleLeaChange(event.target.value)} disabled={!form.provinceCode}><option value="">Select LEA</option>{leas.map((lea) => <option key={lea.leaId} value={lea.leaCode}>{lea.leaCode} · {lea.name}</option>)}</select></label>
        <label>Assigned LEA engineer<select required value={form.assignedEngineerId} onChange={(event) => updateForm('assignedEngineerId', event.target.value)} disabled={!form.leaCode}><option value="">Select engineer</option>{engineers.map((employee) => <option key={employee.userId} value={employee.userId}>{employee.fullName}</option>)}</select></label>
        <label>Latitude<input required type="number" min="-90" max="90" step="any" value={form.latitude} onChange={(event) => updateForm('latitude', event.target.value)} /></label>
        <label>Longitude<input required type="number" min="-180" max="180" step="any" value={form.longitude} onChange={(event) => updateForm('longitude', event.target.value)} /></label>
        <div className="form-actions"><button className="secondary-button" type="button" onClick={() => setIsFormOpen(false)}>Cancel</button><button className="primary-button" type="submit" disabled={metadata.loading}>{metadata.loading ? 'Loading assignments…' : 'Create device'}</button></div>
      </form>
    </Panel>}
    <Panel title="Managed devices" description="Inventory is supplied by the topology service; no local fixture records are used."><RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && devices.length === 0} onRetry={resource.reload}><div className="table-wrap"><table className="data-table"><thead><tr><th>Device</th><th>Type</th><th>IP address</th><th>Region</th><th>Province</th><th>LEA</th><th>Priority level</th><th>Status</th></tr></thead><tbody>{devices.map((device) => <tr key={device.deviceId}><td><strong>{device.deviceName}</strong><span className="table-subtext">ID {device.deviceId}</span></td><td>{device.deviceType}</td><td>{device.ip || 'Unavailable'}</td><td>{device.regionCode || 'Unavailable'}</td><td>{device.provinceCode || 'Unavailable'}</td><td>{device.leaCode || 'Unavailable'}</td><td><span className={`priority-badge priority-${device.priorityLevel.toLowerCase()}`}>{device.priorityLevel}</span></td><td><span className={`status-badge ${statusTone(device.status)}`}>{device.status}</span></td></tr>)}</tbody></table></div></RequestState></Panel>
  </>
}

function statusTone(status: string) {
  const normalized = status.toLowerCase()
  if (normalized === 'up') return 'healthy'
  if (normalized === 'down' || normalized === 'unreachable') return 'critical'
  if (normalized === 'impacted') return 'warning'
  return 'neutral'
}
