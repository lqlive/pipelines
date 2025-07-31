import React, { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import {
  HomeIcon,
  FolderIcon,
  Cog6ToothIcon,
  Bars3Icon,
  XMarkIcon,
  RocketLaunchIcon,
} from '@heroicons/react/24/outline';
import classNames from 'classnames';
import type { NavigationItem } from '../../types';

interface LayoutProps {
  children: React.ReactNode;
}

const navigation: NavigationItem[] = [
  { name: 'Dashboard', href: '/', icon: HomeIcon },
  { name: 'Repositories', href: '/repositories', icon: FolderIcon },
  { name: 'Settings', href: '/settings', icon: Cog6ToothIcon },
];

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
  const location = useLocation();

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
    window.location.href = '/';
  };

  const getUserData = () => {
    const userData = localStorage.getItem('user');
    return userData ? JSON.parse(userData) : null;
  };

  const user = getUserData();

  return (
    <div className="min-h-screen bg-white">
      {/* Top Navigation */}
      <header className="sticky top-0 z-50 bg-white border-b border-gray-50">
        <div className="max-w-none px-8">
          <div className="flex h-14 items-center justify-between">
            {/* Left: Logo + Navigation */}
            <div className="flex items-center space-x-8">
              {/* Logo */}
              <div className="flex items-center">
                <RocketLaunchIcon className="h-4 w-4 text-gray-900" />
                <span className="ml-2 text-sm font-medium text-gray-900">Pipelines</span>
              </div>

              {/* Desktop Navigation */}
              <nav className="hidden md:flex items-center space-x-6">
                {navigation.map((item) => (
                  <Link
                    key={item.name}
                    to={item.href}
                    className={classNames(
                      'text-sm font-normal transition-colors duration-100',
                      location.pathname === item.href
                        ? 'text-gray-900'
                        : 'text-gray-500 hover:text-gray-900'
                    )}
                  >
                    {item.name}
                  </Link>
                ))}
              </nav>
            </div>

            {/* Right: User Area */}
            <div className="flex items-center space-x-4">
              {/* Mobile Menu Button */}
              <button
                type="button"
                className="md:hidden -m-2.5 p-2.5 text-gray-700"
                onClick={() => setMobileMenuOpen(true)}
              >
                <Bars3Icon className="h-4 w-4" />
              </button>

              {/* User Info */}
              {(() => {
                const userData = localStorage.getItem('user');
                const user = userData ? JSON.parse(userData) : null;
                
                return user ? (
                  <div className="flex items-center space-x-3">
                    <div className="text-sm text-right hidden sm:block">
                      <div className="text-gray-900 font-medium">{user.name}</div>
                      <div className="text-gray-500 text-xs">{user.email}</div>
                    </div>
                    {user.avatar ? (
                      <img
                        src={user.avatar}
                        alt={user.name}
                        className="h-8 w-8 rounded-full"
                      />
                    ) : (
                      <div className="h-8 w-8 bg-gray-900 rounded-full flex items-center justify-center">
                        <span className="text-xs text-white font-medium">
                          {user.name?.charAt(0) || 'U'}
                        </span>
                      </div>
                    )}
                    <button
                      onClick={() => {
                        localStorage.removeItem('authToken');
                        localStorage.removeItem('user');
                        window.location.href = '/';
                      }}
                      className="text-sm text-gray-500 hover:text-gray-900 transition-colors hidden sm:block"
                    >
                      Logout
                    </button>
                  </div>
                ) : (
                  <div className="h-8 w-8 bg-gray-900 rounded-full flex items-center justify-center">
                    <span className="text-xs text-white font-medium">U</span>
                  </div>
                );
              })()}
            </div>
          </div>
        </div>
      </header>

      {/* Mobile Menu */}
      {mobileMenuOpen && (
        <div className="md:hidden fixed inset-0 z-50">
          <div className="fixed inset-0 bg-gray-600 bg-opacity-50" onClick={() => setMobileMenuOpen(false)} />
          <div className="fixed top-0 right-0 w-full max-w-sm h-full bg-white shadow-lg">
            <div className="flex items-center justify-between h-14 px-6 border-b border-gray-100">
              <div className="flex items-center">
                <RocketLaunchIcon className="h-4 w-4 text-gray-900" />
                <span className="ml-2 text-sm font-medium text-gray-900">Pipelines</span>
              </div>
              <button
                onClick={() => setMobileMenuOpen(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                <XMarkIcon className="h-4 w-4" />
              </button>
            </div>
            <nav className="p-6">
              {navigation.map((item) => (
                <Link
                  key={item.name}
                  to={item.href}
                  className={classNames(
                    'block px-3 py-3 text-sm font-normal rounded-md transition-colors duration-100',
                    location.pathname === item.href
                      ? 'bg-gray-100 text-gray-900'
                      : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                  )}
                  onClick={() => setMobileMenuOpen(false)}
                >
                  {item.name}
                </Link>
              ))}
            </nav>
          </div>
        </div>
      )}

      {/* Main Content Area */}
      <main className="px-8 py-8">
        <div className="max-w-7xl mx-auto">
          {children}
        </div>
      </main>
    </div>
  );
};

export default Layout; 