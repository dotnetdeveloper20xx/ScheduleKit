import { useState } from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from './Sidebar';
import { Header } from './Header';

export function MainLayout() {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  return (
    <div className="flex h-screen bg-gray-50 dark:bg-gray-900">
      {/* Mobile sidebar overlay */}
      {isMobileMenuOpen && (
        <div
          className="fixed inset-0 z-40 bg-gray-600 bg-opacity-50 lg:hidden"
          onClick={() => setIsMobileMenuOpen(false)}
        />
      )}

      {/* Mobile sidebar */}
      <div
        className={`fixed inset-y-0 left-0 z-50 w-64 transform bg-white transition-transform duration-300 ease-in-out lg:hidden ${
          isMobileMenuOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        <Sidebar onClose={() => setIsMobileMenuOpen(false)} />
      </div>

      {/* Desktop sidebar */}
      <div className="hidden lg:block">
        <Sidebar />
      </div>

      <div className="flex flex-1 flex-col overflow-hidden">
        <Header onMenuClick={() => setIsMobileMenuOpen(true)} />
        <main className="flex-1 overflow-y-auto p-4 sm:p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
