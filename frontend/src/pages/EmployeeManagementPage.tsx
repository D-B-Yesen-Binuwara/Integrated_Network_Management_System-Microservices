import { useMemo, useState } from 'react'
import { Icon } from '../components/Icon'
import { PageHeader, Panel } from '../components/PageBits'
import { RequestState } from '../components/StateViews'
import { useApiRequest } from '../hooks/useApiRequest'
import {
  getAccountRequests,
  getEmployees,
  getRoles,
  getTopologyDevices,
  updateAccountRequestStatus,
  type AccountRequestRecord,
  type EmployeeRecord,
  type RoleRecord,
  type TopologyDevice,
} from '../services/networkApi'

type EmployeeSnapshot = {
  employees: EmployeeRecord[]
  roles: RoleRecord[]
  devices: TopologyDevice[]
}

export function EmployeeManagementPage() {
  const employeesRequest = useApiRequest('employees', getEmployees)
  const rolesRequest = useApiRequest('roles', getRoles)
  const requestsRequest = useApiRequest('account-requests', getAccountRequests)
  const devicesRequest = useApiRequest('employee-device-assignments', getTopologyDevices)
  const [busyRequestId, setBusyRequestId] = useState<number | null>(null)
  const [actionError, setActionError] = useState<string | null>(null)

  const reload = () => {
    employeesRequest.reload()
    rolesRequest.reload()
    requestsRequest.reload()
    devicesRequest.reload()
  }

  const snapshot: EmployeeSnapshot = {
    employees: employeesRequest.data ?? [],
    roles: rolesRequest.data ?? [],
    devices: devicesRequest.data ?? [],
  }
  const loading = employeesRequest.loading || rolesRequest.loading || devicesRequest.loading
  const error = employeesRequest.error || rolesRequest.error || devicesRequest.error
  const pendingRequests = useMemo(() => (requestsRequest.data ?? []).filter((request) => request.status === 'PENDING'), [requestsRequest.data])

  // Device ownership is an application-level reference because identity and
  // topology are separate databases; joining here keeps the UI honest.
  const updateRequest = async (request: AccountRequestRecord, status: 'APPROVED' | 'REJECTED') => {
    setBusyRequestId(request.requestId)
    setActionError(null)
    try {
      await updateAccountRequestStatus(request.requestId, status)
      requestsRequest.reload()
      employeesRequest.reload()
    } catch (reason: unknown) {
      setActionError(reason instanceof Error ? reason.message : 'The account request could not be updated.')
    } finally {
      setBusyRequestId(null)
    }
  }

  return <>
    <PageHeader eyebrow="Management / Workforce" title="Employee management" subtitle="Maintain operational ownership across regions, provinces, and LEAs.">
      <button className="secondary-button" type="button" onClick={reload}><Icon name="refresh" />Refresh</button>
    </PageHeader>

    <div className="detail-grid">
      <div className="detail-card"><div className="detail-label">Employees</div><div className="detail-title">{employeesRequest.loading ? 'Loading…' : snapshot.employees.length}</div><p className="detail-copy">Engineers and officers returned by identity service.</p></div>
      <div className="detail-card"><div className="detail-label">Roles</div><div className="detail-title">{rolesRequest.loading ? 'Loading…' : snapshot.roles.length}</div><p className="detail-copy">Configured operational responsibility levels.</p></div>
      <div className="detail-card"><div className="detail-label">Pending accounts</div><div className="detail-title">{requestsRequest.loading ? 'Loading…' : pendingRequests.length}</div><p className="detail-copy">Account requests awaiting an administrative decision.</p></div>
    </div>

    <Panel title="Operational workforce" description="Area codes are read from identity assignments. A device can be assigned to an LEA engineer across multiple devices.">
      <RequestState loading={loading} error={error} empty={!loading && !error && snapshot.employees.length === 0} onRetry={reload}>
        <div className="table-wrap"><table className="data-table"><thead><tr><th>Employee</th><th>Role</th><th>Contact</th><th>Coverage</th><th>Assigned devices</th></tr></thead><tbody>{snapshot.employees.map((employee) => {
          const assignedDevices = snapshot.devices.filter((device) => device.assignedEngineerId === employee.userId).length
          return <tr key={employee.userId}><td><strong>{employee.fullName}</strong><span className="table-subtext">{employee.username}</span></td><td>{employee.roleName ?? `Role ${employee.roleId}`}</td><td>{employee.email ?? employee.serviceId ?? 'Unavailable'}</td><td><span className="code-stack"><span>{employee.regionCode ?? '—'}</span><span>{employee.provinceCode ?? '—'}</span><span>{employee.leaCode ?? '—'}</span></span></td><td><span className="count-pill">{assignedDevices}</span></td></tr>
        })}</tbody></table></div>
      </RequestState>
    </Panel>

    <div className="management-grid">
      <Panel title="Account requests" description="Approve or reject requests after reviewing their requested operational area." className="management-panel">
        {actionError && <div className="state-error management-error"><strong>Action failed</strong><span>{actionError}</span></div>}
        <RequestState loading={requestsRequest.loading} error={requestsRequest.error} empty={!requestsRequest.loading && !requestsRequest.error && (requestsRequest.data?.length ?? 0) === 0} onRetry={requestsRequest.reload}>
          <div className="table-wrap"><table className="data-table"><thead><tr><th>Applicant</th><th>Role</th><th>Area</th><th>Status</th><th>Action</th></tr></thead><tbody>{(requestsRequest.data ?? []).map((request) => <tr key={request.requestId}><td><strong>{request.fullName}</strong><span className="table-subtext">{request.email}</span></td><td>{snapshot.roles.find((role) => role.roleId === request.roleId)?.roleName ?? `Role ${request.roleId}`}</td><td>{request.leaCode ?? request.provinceCode ?? request.regionCode ?? 'Not specified'}</td><td><span className={`status-badge ${request.status === 'APPROVED' ? 'healthy' : request.status === 'REJECTED' ? 'neutral' : 'warning'}`}>{request.status}</span></td><td>{request.status === 'PENDING' ? <span className="inline-actions"><button className="table-action approve" type="button" disabled={busyRequestId === request.requestId} onClick={() => void updateRequest(request, 'APPROVED')}>Approve</button><button className="table-action reject" type="button" disabled={busyRequestId === request.requestId} onClick={() => void updateRequest(request, 'REJECTED')}>Reject</button></span> : <span className="muted-cell">Closed</span>}</td></tr>)}</tbody></table></div>
        </RequestState>
      </Panel>
    </div>
  </>
}
