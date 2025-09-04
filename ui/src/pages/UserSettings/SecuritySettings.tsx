import React, { useState, useEffect } from 'react';
import {
  EyeIcon,
  EyeSlashIcon,
  TrashIcon,
  ExclamationTriangleIcon,
} from '@heroicons/react/24/outline';
import { SessionService, Session } from '../../services/sessionService';

interface SecurityForm {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

interface SecurityTabProps {
  onNotification?: (type: 'success' | 'error', message: string) => void;
}

const SecurityTab: React.FC<SecurityTabProps> = ({ onNotification }) => {
  const [showCurrentPassword, setShowCurrentPassword] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [sessions, setSessions] = useState<Session[]>([]);
  const [loadingSessions, setLoadingSessions] = useState(false);
  const [deletingSessionId, setDeletingSessionId] = useState<string | null>(null);
  const [sessionToDelete, setSessionToDelete] = useState<{ id: string; name: string } | null>(null);

  const [securityForm, setSecurityForm] = useState<SecurityForm>({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });

  const handleSecurityChange = (key: keyof SecurityForm, value: string) => {
    setSecurityForm(prev => ({
      ...prev,
      [key]: value
    }));
  };

  const changePassword = () => {
    if (securityForm.newPassword !== securityForm.confirmPassword) {
      if (onNotification) {
        onNotification('error', 'New passwords do not match');
      }
      return;
    }
    if (securityForm.newPassword.length < 6) {
      if (onNotification) {
        onNotification('error', 'Password must be at least 6 characters');
      }
      return;
    }
    console.log('Change password');
    // TODO: Implement password change logic
    if (onNotification) {
      onNotification('success', 'Password changed successfully');
    }
    setSecurityForm({
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    });
  };

  const enable2FA = () => {
    console.log('Enable 2FA');
    // TODO: Implement 2FA setup
    if (onNotification) {
      onNotification('success', 'Two-factor authentication setup initiated');
    }
  };

  const deleteAccount = () => {
    console.log('Delete account');
    // TODO: Implement account deletion
    if (confirm('Are you absolutely sure you want to delete your account? This action cannot be undone and all your data will be permanently lost.')) {
      if (onNotification) {
        onNotification('success', 'Account deletion initiated');
      }
    }
  };

  const loadSessions = async () => {
    try {
      setLoadingSessions(true);
      const sessionData = await SessionService.getSessions();
      setSessions(sessionData);
    } catch (error) {
      console.error('Failed to load sessions:', error);
      if (onNotification) {
        onNotification('error', 'Failed to load sessions');
      }
    } finally {
      setLoadingSessions(false);
    }
  };

  const handleDeleteSessionRequest = (sessionId: string, deviceName: string, deviceType: string) => {
    const sessionName = `${deviceName} on ${deviceType}`;
    setSessionToDelete({ id: sessionId, name: sessionName });
  };

  const handleConfirmDeleteSession = async () => {
    if (!sessionToDelete) return;

    try {
      setDeletingSessionId(sessionToDelete.id);
      await SessionService.deleteSession(sessionToDelete.id);
      
      // Reload session list
      await loadSessions();
      if (onNotification) {
        onNotification('success', 'Session deleted successfully');
      }
    } catch (error) {
      console.error('Failed to delete session:', error);
      if (onNotification) {
        onNotification('error', 'Failed to delete session. Please try again.');
      }
    } finally {
      setDeletingSessionId(null);
      setSessionToDelete(null);
    }
  };

  const handleCancelDeleteSession = () => {
    setSessionToDelete(null);
  };

  // Load sessions when component mounts
  useEffect(() => {
    loadSessions();
  }, []);

  return (
    <>
      <div className="space-y-8">
          {/* Change Password */}
          <div>
            <h3 className="text-lg font-medium text-gray-900 mb-4">Change Password</h3>
            <div className="space-y-4 max-w-md">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Current Password
                </label>
                <div className="relative">
                  <input
                    type={showCurrentPassword ? 'text' : 'password'}
                    value={securityForm.currentPassword}
                    onChange={(e) => handleSecurityChange('currentPassword', e.target.value)}
                    className="minimal-input pr-10"
                    placeholder="Enter current password"
                  />
                  <button
                    type="button"
                    className="absolute inset-y-0 right-0 pr-3 flex items-center"
                    onClick={() => setShowCurrentPassword(!showCurrentPassword)}
                  >
                    {showCurrentPassword ? (
                      <EyeSlashIcon className="h-4 w-4 text-gray-400" />
                    ) : (
                      <EyeIcon className="h-4 w-4 text-gray-400" />
                    )}
                  </button>
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  New Password
                </label>
                <div className="relative">
                  <input
                    type={showNewPassword ? 'text' : 'password'}
                    value={securityForm.newPassword}
                    onChange={(e) => handleSecurityChange('newPassword', e.target.value)}
                    className="minimal-input pr-10"
                    placeholder="Enter new password"
                  />
                  <button
                    type="button"
                    className="absolute inset-y-0 right-0 pr-3 flex items-center"
                    onClick={() => setShowNewPassword(!showNewPassword)}
                  >
                    {showNewPassword ? (
                      <EyeSlashIcon className="h-4 w-4 text-gray-400" />
                    ) : (
                      <EyeIcon className="h-4 w-4 text-gray-400" />
                    )}
                  </button>
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Confirm New Password
                </label>
                <div className="relative">
                  <input
                    type={showConfirmPassword ? 'text' : 'password'}
                    value={securityForm.confirmPassword}
                    onChange={(e) => handleSecurityChange('confirmPassword', e.target.value)}
                    className="minimal-input pr-10"
                    placeholder="Confirm new password"
                  />
                  <button
                    type="button"
                    className="absolute inset-y-0 right-0 pr-3 flex items-center"
                    onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  >
                    {showConfirmPassword ? (
                      <EyeSlashIcon className="h-4 w-4 text-gray-400" />
                    ) : (
                      <EyeIcon className="h-4 w-4 text-gray-400" />
                    )}
                  </button>
                </div>
                {securityForm.newPassword && securityForm.confirmPassword && 
                 securityForm.newPassword !== securityForm.confirmPassword && (
                  <p className="text-xs text-red-600 mt-1">Passwords do not match</p>
                )}
              </div>
              <button
                onClick={changePassword}
                disabled={!securityForm.currentPassword || !securityForm.newPassword || 
                         securityForm.newPassword !== securityForm.confirmPassword}
                className="btn-primary disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Change Password
              </button>
            </div>
          </div>

          {/* Two-Factor Authentication */}
          <div className="pt-6 border-t border-gray-200">
            <h3 className="text-lg font-medium text-gray-900 mb-4">Two-Factor Authentication</h3>
            <div className="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
              <div>
                <div className="text-sm font-medium text-gray-900">Two-factor authentication</div>
                <div className="text-sm text-gray-500">Add an extra layer of security to your account</div>
              </div>
              <button onClick={enable2FA} className="btn-secondary text-sm">
                Enable 2FA
              </button>
            </div>
          </div>

          {/* Active Sessions */}
          <div className="pt-6 border-t border-gray-200">
            <h3 className="text-lg font-medium text-gray-900 mb-4">Active Sessions</h3>
            
            {loadingSessions ? (
              <div className="flex items-center justify-center py-8">
                <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600"></div>
                <span className="ml-2 text-sm text-gray-500">Loading sessions...</span>
              </div>
            ) : (
              <div className="space-y-3">
                {sessions.length === 0 ? (
                  <div className="text-sm text-gray-500 py-4">No active sessions found.</div>
                ) : (
                  sessions.map((session) => {
                    const isCurrentDevice = session.isCurrent;
                    const deviceType = session.deviceType ?? 'Unknown';
                    const deviceName = session.deviceName ?? 'Unknown';
                    const lastActive = SessionService.formatLastActive(session.lastActiveAt);
                    
                    return (
                      <div key={session.sessionToken} className="flex items-center justify-between p-3 border border-gray-200 rounded-lg">
                        <div className="flex-1">
                          <div className="flex items-center space-x-2">
                            <div className="text-sm font-medium text-gray-900">
                              {deviceName} on {deviceType}
                            </div>
                            {isCurrentDevice && (
                              <span className="text-xs text-green-600 font-medium bg-green-100 px-2 py-1 rounded">
                                Current
                              </span>
                            )}
                          </div>
                          <div className="text-sm text-gray-500 mt-1">
                            {session.ipAddress && (
                              <span>IP: {session.ipAddress} • </span>
                            )}
                            {lastActive}
                            {session.location && (
                              <span> • {session.location}</span>
                            )}
                          </div>
                          <div className="text-xs text-gray-400 mt-1">
                            Created: {new Date(session.createdAt).toLocaleDateString()} • 
                            Expires: {new Date(session.expiresAt).toLocaleDateString()}
                          </div>
                        </div>
                        {!isCurrentDevice && (
                          <div className="ml-4">
                            <button
                              onClick={() => handleDeleteSessionRequest(session.id, deviceName, deviceType)}
                              disabled={deletingSessionId === session.id}
                              className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                              title="Delete Session"
                            >
                              {deletingSessionId === session.id ? (
                                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-red-600"></div>
                              ) : (
                                <TrashIcon className="h-4 w-4" />
                              )}
                            </button>
                          </div>
                        )}
                      </div>
                    );
                  })
                )}
              </div>
            )}
          </div>

          {/* Danger Zone */}
          <div className="pt-8 border-t border-gray-200">
            <h3 className="text-lg font-medium text-red-600 mb-4">Danger Zone</h3>
            <div className="bg-red-50 border border-red-200 rounded-lg p-6">
              <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
                <div className="mb-4 sm:mb-0">
                  <h4 className="text-sm font-medium text-red-800">Delete Account</h4>
                  <p className="text-sm text-red-700 mt-1">
                    Permanently delete your account and all associated data.<br/>
                    This action cannot be undone.
                  </p>
                </div>
                <button onClick={deleteAccount} className="btn-danger shrink-0">
                  Delete Account
                </button>
              </div>
            </div>
          </div>
      </div>

      {/* Confirmation Modal */}
      {sessionToDelete && (
        <div className="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-sm w-full mx-4">
            <div className="flex items-center mb-4">
              <ExclamationTriangleIcon className="h-6 w-6 text-red-600 mr-3" />
              <h3 className="text-lg font-medium text-gray-900">Confirm Delete Session</h3>
            </div>
            <p className="text-sm text-gray-500 mb-6">
              Are you sure you want to delete the session for <strong>{sessionToDelete.name}</strong>? 
              This will log out that device and cannot be undone.
            </p>
            <div className="flex justify-end space-x-3">
              <button
                onClick={handleCancelDeleteSession}
                className="btn-secondary px-4 py-2"
              >
                Cancel
              </button>
              <button
                onClick={handleConfirmDeleteSession}
                disabled={deletingSessionId === sessionToDelete.id}
                className="btn-danger px-4 py-2 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {deletingSessionId === sessionToDelete.id ? 'Deleting...' : 'Delete Session'}
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default SecurityTab;
