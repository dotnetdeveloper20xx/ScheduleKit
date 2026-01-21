import { Routes, Route } from 'react-router-dom'
import { AuthProvider } from '@/contexts/AuthContext'
import { ThemeProvider } from '@/contexts/ThemeContext'
import { MainLayout } from '@/components/layout/MainLayout'
import { ProtectedRoute } from '@/components/layout/ProtectedRoute'
import { EventTypeListPage } from '@/features/event-types/EventTypeListPage'
import { EventTypeCreatePage } from '@/features/event-types/EventTypeCreatePage'
import { EventTypeEditPage } from '@/features/event-types/EventTypeEditPage'
import { DashboardPage } from '@/features/dashboard/DashboardPage'
import { AvailabilityPage } from '@/features/availability/AvailabilityPage'
import { BookingsListPage, BookingDetailPage } from '@/features/bookings'
import { PublicBookingPage } from '@/features/booking/PublicBookingPage'
import { LoginPage, RegisterPage } from '@/features/auth'
import { SettingsPage } from '@/features/settings'

function App() {
  return (
    <ThemeProvider>
      <AuthProvider>
        <Routes>
          {/* Auth Routes */}
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* Protected Host Dashboard Routes */}
          <Route element={<ProtectedRoute />}>
            <Route element={<MainLayout />}>
              <Route path="/" element={<DashboardPage />} />
              <Route path="/event-types" element={<EventTypeListPage />} />
              <Route path="/event-types/new" element={<EventTypeCreatePage />} />
              <Route path="/event-types/:id/edit" element={<EventTypeEditPage />} />
              <Route path="/availability" element={<AvailabilityPage />} />
              <Route path="/bookings" element={<BookingsListPage />} />
              <Route path="/bookings/:id" element={<BookingDetailPage />} />
              <Route path="/settings" element={<SettingsPage />} />
            </Route>
          </Route>

          {/* Public Booking Routes */}
          <Route path="/book/:hostSlug/:eventSlug" element={<PublicBookingPage />} />
        </Routes>
      </AuthProvider>
    </ThemeProvider>
  )
}

export default App
