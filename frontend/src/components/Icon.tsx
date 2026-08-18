import type { SVGProps } from 'react'

type IconName = 'activity' | 'alarm' | 'analytics' | 'bell' | 'box' | 'chevron' | 'circle' | 'dashboard' | 'device' | 'download' | 'layers' | 'link' | 'map' | 'refresh' | 'search' | 'settings' | 'spark' | 'topology' | 'users'

const paths: Record<IconName, string> = {
  activity: 'M3 12h4l2-7 4 14 2-7h6',
  alarm: 'M18 8a6 6 0 0 0-12 0c0 7-3 7-3 9h18c0-2-3-2-3-9M10 21h4',
  analytics: 'M4 19V5m0 14h17M8 16v-4m4 4V8m4 8v-6m4 6V5',
  bell: 'M18 8a6 6 0 0 0-12 0c0 7-2 7-2 9h16c0-2-2-2-2-9M10 21h4',
  box: 'M4 7 12 3l8 4-8 4-8-4Zm0 0v10l8 4 8-4V7M12 11v10',
  chevron: 'm9 6 6 6-6 6',
  circle: 'M12 12h.01M12 3a9 9 0 1 0 0 18 9 9 0 0 0 0-18Z',
  dashboard: 'M4 4h6v6H4zm10 0h6v6h-6zM4 14h6v6H4zm10 0h6v6h-6z',
  device: 'M5 5h14v14H5zM9 9h6v6H9zM9 2v3m6-3v3m-6 14v3m6-3v3M2 9h3m-3 6h3m14-6h3m-3 6h3',
  download: 'M12 3v12m0 0 5-5m-5 5-5-5M5 21h14',
  layers: 'm12 3 9 5-9 5-9-5 9-5Zm-9 9 9 5 9-5m-18 5 9 5 9-5',
  link: 'M10 13a5 5 0 0 0 7.54.54l2-2a5 5 0 0 0-7.07-7.07l-1.15 1.15m-1.32 7.7a5 5 0 0 0-7.54-.54l-2 2a5 5 0 0 0 7.07 7.07l1.15-1.15',
  map: 'M3 6 9 3l6 3 6-3v15l-6 3-6-3-6 3V6Zm6-3v15m6-12v15',
  refresh: 'M20 11a8.1 8.1 0 0 0-14.7-4L3 10m0 0V4m0 6h6M4 13a8.1 8.1 0 0 0 14.7 4L21 14m0 0v6m0-6h-6',
  search: 'm21 21-4.3-4.3M10.8 18a7.2 7.2 0 1 1 0-14.4 7.2 7.2 0 0 1 0 14.4Z',
  settings: 'M12 15.5a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7Zm7.4-3.5 1.4 1.1-1.8 3.1-1.7-.7-1.5.9-.2 1.8h-3.6l-.2-1.8-1.5-.9-1.7.7-1.8-3.1L8.2 12l-.1-1.8-1.5-1.1 1.8-3.1 1.7.7 1.5-.9.2-1.8h3.6l.2 1.8 1.5.9 1.7-.7 1.8 3.1-1.5 1.1.3 1.8Z',
  spark: 'm12 3 1.5 6.5L20 11l-6.5 1.5L12 19l-1.5-6.5L4 11l6.5-1.5L12 3Z',
  topology: 'M7 5a2 2 0 1 0 0 4 2 2 0 0 0 0-4Zm10 10a2 2 0 1 0 0 4 2 2 0 0 0 0-4ZM7 9v5a4 4 0 0 0 4 4h4M17 15V9a4 4 0 0 0-4-4h-4',
  users: 'M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2m7-10a4 4 0 1 0 0-8 4 4 0 0 0 0 8Zm7-8a4 4 0 0 1 0 7.8M22 21v-2a4 4 0 0 0-3-3.87',
}

export function Icon({ name, ...props }: { name: IconName } & SVGProps<SVGSVGElement>) {
  return <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true" {...props}><path d={paths[name]} /></svg>
}
