import React from 'react';
import { BrowserRouter as Router, Routes, Route, useLocation, Navigate } from 'react-router-dom';
import Layout from './components/Layout/Layout';
import Dashboard from './pages/Dashboard';
import Repositories from './pages/Repositories';
import ImportRepository from './pages/NewRepository';
import Repository from './pages/Repository';
import RepositorySettings from './pages/RepositorySettings';
import Build from './pages/Build';
import Settings from './pages/Settings';
import UserProfile from './pages/UserProfile';
import Login from './pages/Login';
import Register from './pages/Register';
import { AuthProvider, useAuth } from './contexts/AuthContext';
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

const AppContent: React.FC = () => {

  return (
    <Router>
      <Routes>
        {/* Public routes */}
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        
        {/* Protected routes */}
        <Route path="/*" element={
          <RequireAuth>
            <Layout>
              <Routes>
                <Route path="/" element={<Dashboard />} />
                <Route path="/repositories" element={<Repositories />} />
                <Route path="/repositories/new" element={<ImportRepository />} />
                <Route path="/repositories/:owner/:name" element={<Repository />} />
                <Route path="/repositories/:owner/:name/settings" element={<RepositorySettings />} />
                <Route path="/repositories/:owner/:name/builds/:buildNumber" element={<Build />} />
                <Route path="/settings" element={<Settings />} />
                <Route path="/profile" element={<UserProfile />} />
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
      <AppContent />
    </AuthProvider>
  );
};

export default App; 