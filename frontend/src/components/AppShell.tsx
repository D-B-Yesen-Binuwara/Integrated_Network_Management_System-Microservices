import { NavLink, Outlet, useLocation } from 'react-router-dom'
import { Icon } from './Icon'

type NavigationItem = { to: string; label: string; icon: 'activity' | 'alarm' | 'analytics' | 'dashboard' | 'device' | 'map' | 'topology' | 'users'; end?: boolean }

const mainNavigation: NavigationItem[] = [
  { to: '/', label: 'Overview', icon: 'dashboard' as const, end: true },
  { to: '/alarms', label: 'Alarm center', icon: 'alarm' as const },
  { to: '/correlation', label: 'Correlation', icon: 'activity' as const },
]
const networkNavigation: NavigationItem[] = [
  { to: '/topology', label: 'Topology map', icon: 'topology' as const },
  { to: '/inventory', label: 'Device inventory', icon: 'device' as const },
  { to: '/analytics', label: 'Analytics', icon: 'analytics' as const },
]
const managementNavigation: NavigationItem[] = [
  { to: '/regions', label: 'Region management', icon: 'map' as const },
  { to: '/employees', label: 'Employees & accounts', icon: 'users' as const },
]

function NavigationLinks({ links }: { links: NavigationItem[] }) {
  return <>{links.map((item) => <NavLink key={item.to} to={item.to} end={item.end} className={({ isActive }) => `nav-link${isActive ? ' active' : ''}`}><Icon name={item.icon} /><span>{item.label}</span></NavLink>)}</>
}

export function AppShell() {
  const location = useLocation()
  const currentLabel = [...mainNavigation, ...networkNavigation, ...managementNavigation].find((item) => item.to === location.pathname)?.label ?? 'Overview'

  return <div className="app-shell">
    <aside className="sidebar">
      <NavLink className="brand" to="/">
        <span className="brand-mark"><Icon name="topology" /></span>
        <span><span className="brand-name">INMS</span><span className="brand-subtitle">Network intelligence</span></span>
      </NavLink>
      <nav className="nav-group" aria-label="Main navigation">
        <div className="nav-label">Command center</div><NavigationLinks links={mainNavigation} />
      </nav>
      <nav className="nav-group" aria-label="Network navigation">
        <div className="nav-label">Network intelligence</div><NavigationLinks links={networkNavigation} />
      </nav>
      <nav className="nav-group" aria-label="Management navigation">
        <div className="nav-label">Management</div><NavigationLinks links={managementNavigation} />
      </nav>
      <nav className="mobile-nav" aria-label="Mobile navigation"><NavigationLinks links={[...mainNavigation, ...networkNavigation, ...managementNavigation]} /></nav>
      <div className="sidebar-footer"><div className="connection-card"><div className="connection-top"><span className="connection-title">Service connection</span><span className="connection-status live">Gateway live</span></div><p className="connection-copy">Live data is loaded through the configured API gateway.</p></div></div>
    </aside>
    <div className="main-area">
      <header className="topbar"><div className="breadcrumb"><span>Workspace</span><Icon name="chevron" /><strong>{currentLabel}</strong></div><div className="topbar-actions"><button className="icon-button" type="button" aria-label="Notifications"><Icon name="bell" /></button><div className="user-chip"><span className="avatar">NO</span><span className="user-name">NOC operator<span className="user-role">Preview workspace</span></span></div></div></header>
      <main className="content"><Outlet /><div className="footer-note">INMS workspace · Operational data is supplied by the gateway and its microservices</div></main>
    </div>
  </div>
}
