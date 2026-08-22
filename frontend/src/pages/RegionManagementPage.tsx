import { useCallback, useMemo } from 'react'
import { Icon } from '../components/Icon'
import { PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiRequest } from '../hooks/useApiRequest'
import { getEmployees, getLeas, getProvinces, getRegions, getTopologyDevices, type EmployeeRecord, type LeaRecord, type ProvinceRecord, type RegionRecord, type TopologyDevice } from '../services/networkApi'

type RegionSnapshot = {
  regions: RegionRecord[]
  provinces: ProvinceRecord[]
  leas: LeaRecord[]
  employees: EmployeeRecord[]
  devices: TopologyDevice[]
}

export function RegionManagementPage() {
  const load = useCallback(async (signal: AbortSignal): Promise<RegionSnapshot> => {
    const [regions, provinces, leas, employees, devices] = await Promise.all([
      getRegions(signal),
      getProvinces(signal),
      getLeas(signal),
      getEmployees(signal),
      getTopologyDevices(signal),
    ])
    return { regions, provinces, leas, employees, devices }
  }, [])
  const resource = useApiRequest('region-management', load)
  const data = resource.data
  const rows = useMemo(() => {
    if (!data) return []
    return data.leas.map((lea) => {
      const province = data.provinces.find((item) => item.provinceId === lea.provinceId) ?? lea.province
      const region = data.regions.find((item) => item.regionId === province?.regionId) ?? province?.region
      const devices = data.devices.filter((device) => device.leaCode === lea.leaCode)
      const engineers = data.employees.filter((employee) => employee.leaCode === lea.leaCode && isLeaEngineer(employee.roleName))
      return { lea, province, region, devices, engineers }
    })
  }, [data])
  const uncoveredLeas = rows.filter((row) => row.engineers.length === 0)

  return <>
    <PageHeader eyebrow="Management / Coverage" title="Region management" subtitle="Inspect the Region → Province → LEA hierarchy and verify engineer coverage.">
      <button className="secondary-button" type="button" onClick={resource.reload}><Icon name="refresh" />Refresh</button>
    </PageHeader>
    <div className="detail-grid">
      <div className="detail-card"><div className="detail-label">Regions</div><div className="detail-title">{resource.loading ? 'Loading…' : data?.regions.length ?? 0}</div><p className="detail-copy">Top-level operational territories.</p></div>
      <div className="detail-card"><div className="detail-label">Provinces</div><div className="detail-title">{resource.loading ? 'Loading…' : data?.provinces.length ?? 0}</div><p className="detail-copy">Provinces inherit their region code.</p></div>
      <div className="detail-card"><div className="detail-label">LEAs</div><div className="detail-title">{resource.loading ? 'Loading…' : data?.leas.length ?? 0}</div><p className="detail-copy">Every device is assigned at this level.</p></div>
    </div>
    <Panel title="LEA coverage" description="A green coverage badge confirms that at least one LEA engineer is assigned. One engineer may own multiple devices.">
      <RequestState loading={resource.loading} error={resource.error} empty={!resource.loading && !resource.error && rows.length === 0} onRetry={resource.reload}>
        <div className="table-wrap"><table className="data-table"><thead><tr><th>Region</th><th>Province</th><th>LEA</th><th>Devices</th><th>LEA engineers</th><th>Coverage</th></tr></thead><tbody>{rows.map((row) => <tr key={row.lea.leaId}><td><strong>{row.region?.regionCode ?? '—'}</strong><span className="table-subtext">{row.region?.name ?? 'Unknown region'}</span></td><td><strong>{row.province?.provinceCode ?? '—'}</strong><span className="table-subtext">{row.province?.name ?? 'Unknown province'}</span></td><td><strong>{row.lea.leaCode}</strong><span className="table-subtext">{row.lea.name}</span></td><td><span className="count-pill">{row.devices.length}</span></td><td><span className="count-pill">{row.engineers.length}</span></td><td><span className={`status-badge ${row.engineers.length > 0 ? 'healthy' : 'critical'}`}>{row.engineers.length > 0 ? 'Covered' : 'Unassigned'}</span></td></tr>)}</tbody></table></div>
      </RequestState>
    </Panel>
    {!resource.loading && !resource.error && uncoveredLeas.length > 0 && <div className="state-error management-error"><strong>Coverage needs attention</strong><span>{uncoveredLeas.length} LEA{uncoveredLeas.length === 1 ? '' : 's'} do not have a LEA engineer assignment.</span></div>}
  </>
}

function isLeaEngineer(roleName: string | null) {
  return (roleName ?? '').toLowerCase().includes('lea')
}
