import { NavLink, Outlet, useLocation } from 'react-router-dom'
import { Icon } from './Icon'

type NavigationItem = { to: string; label: string; icon: 'activity' | 'alarm' | 'analytics' | 'dashboard' | 'device' | 'topology'; end?: boolean }

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

function NavigationLinks({ links }: { links: NavigationItem[] }) {
  return <>{links.map((item) => <NavLink key={item.to} to={item.to} end={item.end} className={({ isActive }) => `nav-link${isActive ? ' active' : ''}`}><Icon name={item.icon} /><span>{item.label}</span></NavLink>)}</>
}

export function AppShell() {
  const location = useLocation()
  const currentLabel = [...mainNavigation, ...networkNavigation].find((item) => item.to === location.pathname)?.label ?? 'Overview'

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
      <nav className="mobile-nav" aria-label="Mobile navigation"><NavigationLinks links={[...mainNavigation, ...networkNavigation]} /></nav>
      <div className="sidebar-footer"><div className="connection-card"><div className="connection-top"><span className="connection-title">Service connection</span><span className="connection-status">Preview</span></div><p className="connection-copy">Backend data is intentionally disconnected for this frontend build.</p></div></div>
    </aside>
    <div className="main-area">
      <header className="topbar"><div className="breadcrumb"><span>Workspace</span><Icon name="chevron" /><strong>{currentLabel}</strong></div><div className="topbar-actions"><button className="icon-button" type="button" aria-label="Notifications"><Icon name="bell" /></button><div className="user-chip"><span className="avatar">NO</span><span className="user-name">NOC operator<span className="user-role">Preview workspace</span></span></div></div></header>
      <main className="content"><Outlet /><div className="footer-note">INMS preview workspace · Connect a gateway to activate live network intelligence</div></main>
    </div>
  </div>
}
