import React, { useState, useRef, useEffect } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import {
  HomeIcon,
  FolderIcon,
  Cog6ToothIcon,
  Bars3Icon,
  XMarkIcon,
  RocketLaunchIcon,
  ChevronDownIcon,
} from '@heroicons/react/24/outline';
import classNames from 'classnames';
import type { NavigationItem } from '../../types';
import { useAuth } from '../../contexts/AuthContext';

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
  const [userMenuOpen, setUserMenuOpen] = useState(false);
  const userMenuRef = useRef<HTMLDivElement>(null);
  const location = useLocation();
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const handleLogout = async () => {
    try {
      await logout();
      navigate('/login', { replace: true });
    } catch (error) {
      console.error('Logout error:', error);
      // Force navigation to login even if logout API fails
      navigate('/login', { replace: true });
    }
  };

  // Close user menu when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (userMenuRef.current && !userMenuRef.current.contains(event.target as Node)) {
        setUserMenuOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, []);

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

              {/* User Menu */}
              {user ? (
                <div className="relative" ref={userMenuRef}>
                  <button
                    onClick={() => setUserMenuOpen(!userMenuOpen)}
                    className="flex items-center space-x-3 px-2 py-1.5 rounded-lg hover:bg-gray-50 transition-all duration-200 group"
                  >
                    <div className="text-sm text-right hidden sm:block">
                      <div className="text-gray-900 font-medium group-hover:text-gray-700 transition-colors">
                        {user.name}
                      </div>
                      <div className="text-gray-500 text-xs">
                        {user.email}
                      </div>
                    </div>
                    {user.avatar ? (
                      <img
                        src={user.avatar}
                        alt={user.name}
                        className="h-9 w-9 rounded-full border-2 border-white shadow-sm"
                      />
                    ) : (
                      <div className="h-9 w-9 bg-gradient-to-br from-blue-500 to-blue-600 rounded-full flex items-center justify-center shadow-sm">
                        <span className="text-sm text-white font-medium">
                          {user.name?.charAt(0) || 'U'}
                        </span>
                      </div>
                    )}
                    <ChevronDownIcon 
                      className={`h-3 w-3 text-gray-400 transition-all duration-200 ${
                        userMenuOpen ? 'rotate-180 text-gray-600' : 'group-hover:text-gray-600'
                      }`} 
                    />
                  </button>

                  {/* Dropdown Menu */}
                  {userMenuOpen && (
                    <div className="absolute right-0 mt-3 w-56 bg-white rounded-xl shadow-xl border border-gray-100 py-2 z-50 animate-in slide-in-from-top-2 duration-200">
                      {/* User Info Header */}
                      <div className="px-4 py-3 border-b border-gray-100">
                        <div className="flex items-center space-x-3">
                          {user.avatar ? (
                            <img
                              src={user.avatar}
                              alt={user.name}
                              className="h-10 w-10 rounded-full"
                            />
                          ) : (
                            <div className="h-10 w-10 bg-gradient-to-br from-blue-500 to-blue-600 rounded-full flex items-center justify-center">
                              <span className="text-sm text-white font-medium">
                                {user.name?.charAt(0) || 'U'}
                              </span>
                            </div>
                          )}
                          <div className="flex-1 min-w-0">
                            <div className="text-sm font-medium text-gray-900 truncate">
                              {user.name}
                            </div>
                            <div className="text-xs text-gray-500 truncate">
                              {user.email}
                            </div>
                          </div>
                        </div>
                      </div>

                      {/* Menu Items */}
                      <div className="py-1">
                        <Link
                          to="/profile"
                          onClick={() => setUserMenuOpen(false)}
                          className="block px-4 py-2.5 text-sm text-gray-700 hover:bg-gray-50 transition-colors duration-150"
                        >
                          Profile Settings
                        </Link>
                        <button
                          onClick={handleLogout}
                          className="block w-full text-left px-4 py-2.5 text-sm text-gray-700 hover:bg-gray-50 transition-colors duration-150"
                        >
                          Sign Out
                        </button>
                      </div>
                    </div>
                  )}
                </div>
              ) : (
                <div className="h-9 w-9 bg-gradient-to-br from-gray-600 to-gray-700 rounded-full flex items-center justify-center shadow-sm">
                  <span className="text-sm text-white font-medium">U</span>
                </div>
              )}
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
              
              {/* Mobile User Menu */}
              {user && (
                <div className="pt-4 mt-4 border-t border-gray-200">
                  <div className="px-3 py-2 text-xs text-gray-500 uppercase tracking-wide font-medium">
                    Account
                  </div>
                  <Link
                    to="/profile"
                    onClick={() => setMobileMenuOpen(false)}
                    className="block px-3 py-3 text-sm font-normal rounded-md text-gray-600 hover:bg-gray-50 hover:text-gray-900 transition-colors"
                  >
                    Profile Settings
                  </Link>
                  <button
                    onClick={async () => {
                      setMobileMenuOpen(false);
                      await handleLogout();
                    }}
                    className="block w-full text-left px-3 py-3 text-sm font-normal rounded-md text-gray-600 hover:bg-gray-50 hover:text-gray-900 transition-colors"
                  >
                    Sign Out
                  </button>
                </div>
              )}
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