import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from './components/Layout/Layout';
import Dashboard from './pages/Dashboard';
import Repositories from './pages/Repositories';
import ImportRepository from './pages/NewRepository';
import Repository from './pages/Repository';
import RepositorySettings from './pages/RepositorySettings';
import Build from './pages/Build';
import Settings from './pages/Settings';
import Login from './pages/Login';
import './App.css';



const App: React.FC = () => {
  // Check if user is authenticated
  const isAuthenticated = () => {
    return localStorage.getItem('authToken') !== null;
  };

  return (
    <Router>
      {!isAuthenticated() ? (
        <Routes>
          <Route path="/*" element={<Login />} />
        </Routes>
      ) : (
        <Layout>
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/repositories" element={<Repositories />} />
            <Route path="/repositories/new" element={<ImportRepository />} />
            <Route path="/repositories/:owner/:name" element={<Repository />} />
            <Route path="/repositories/:owner/:name/settings" element={<RepositorySettings />} />
            <Route path="/repositories/:owner/:name/builds/:buildNumber" element={<Build />} />
            <Route path="/settings" element={<Settings />} />
          </Routes>
        </Layout>
      )}
    </Router>
  );
};

export default App; 