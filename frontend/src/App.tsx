import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import './App.css'
import { AppShell } from './components/AppShell'
import { AlarmsPage } from './pages/AlarmsPage'
import { AnalyticsPage } from './pages/AnalyticsPage'
import { CorrelationPage } from './pages/CorrelationPage'
import { DashboardPage } from './pages/DashboardPage'
import { InventoryPage } from './pages/InventoryPage'
import { EmployeeManagementPage } from './pages/EmployeeManagementPage'
import { RegionManagementPage } from './pages/RegionManagementPage'
import { TopologyPage } from './pages/TopologyPage'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<AppShell />}>
          <Route index element={<DashboardPage />} />
          <Route path="alarms" element={<AlarmsPage />} />
          <Route path="correlation" element={<CorrelationPage />} />
          <Route path="topology" element={<TopologyPage />} />
          <Route path="inventory" element={<InventoryPage />} />
          <Route path="analytics" element={<AnalyticsPage />} />
          <Route path="employees" element={<EmployeeManagementPage />} />
          <Route path="regions" element={<RegionManagementPage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}

export default App
