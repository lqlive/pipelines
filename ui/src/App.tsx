import React from 'react';
import { BrowserRouter as Router, Routes, Route, useLocation, Navigate } from 'react-router-dom';
import Layout from './components/Layout/Layout';
import Dashboard from './pages/Dashboard';
import Repositories from './pages/Repositories';
import ImportRepository from './pages/NewRepository';
import Repository from './pages/Repositories/Repository';
import RepositorySettings from './pages/Repositories/RepositorySettings';
import Build from './pages/Build';
import Settings from './pages/Settings';
import UserProfile from './pages/UserSettings/UserProfile';
import Login from './pages/Login';
import Register from './pages/Register';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { NotificationProvider } from './contexts/NotificationContext';
import './App.css';



// Component to handle redirect logic for unauthenticated users
const RequireAuth: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (!isAuthenticated) {
    // Redirect to login with the current location as redirectUri
    const redirectUri = encodeURIComponent(location.pathname + location.search);
    return <Navigate to={`/login?redirectUri=${redirectUri}`} replace />;
  }

  return <>{children}</>;
};

// Component to prevent authenticated users from accessing auth pages
const RequireNoAuth: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (isAuthenticated) {
    // Redirect authenticated users to dashboard
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
};

const AppContent: React.FC = () => {

  return (
    <Router>
      <Routes>
        {/* Auth routes - redirect if already authenticated */}
        <Route path="/login" element={
          <RequireNoAuth>
            <Login />
          </RequireNoAuth>
        } />
        <Route path="/register" element={
          <RequireNoAuth>
            <Register />
          </RequireNoAuth>
        } />
        
        {/* Protected routes */}
        <Route path="/*" element={
          <RequireAuth>
            <Layout>
              <Routes>
                <Route path="/" element={<Dashboard />} />
                <Route path="/repositories" element={<Repositories />} />
                <Route path="/repositories/new" element={<ImportRepository />} />
                <Route path="/repositories/:owner/:name" element={<Repository />} />
                <Route path="/repositories/:owner/:name/settings/*" element={<RepositorySettings />} />
                <Route path="/repositories/:owner/:name/builds/:buildNumber" element={<Build />} />
                <Route path="/settings" element={<Settings />} />
                <Route path="/profile/*" element={<UserProfile />} />
              </Routes>
            </Layout>
          </RequireAuth>
        } />
      </Routes>
    </Router>
  );
};

const App: React.FC = () => {
  return (
    <AuthProvider>
      <NotificationProvider>
        <AppContent />
      </NotificationProvider>
    </AuthProvider>
  );
};

export default App; 