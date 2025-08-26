import React, { useState, useEffect } from 'react';
import {
  UserIcon,
  CameraIcon,
  EyeIcon,
  EyeSlashIcon,
  EnvelopeIcon,
  BellIcon,
  ShieldCheckIcon,
} from '@heroicons/react/24/outline';
import { Link } from 'react-router-dom';
import { UserService } from '../services/userService';
import { SessionService, Session } from '../services/sessionService';

type TabId = 'profile' | 'account' | 'security' | 'notifications';

interface Tab {
  id: TabId;
  name: string;
  icon: React.ComponentType<React.SVGProps<SVGSVGElement>>;
}

interface ProfileForm {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  bio: string;
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

const UserProfile: React.FC = () => {
  const [activeTab, setActiveTab] = useState<TabId>('profile');
  const [showCurrentPassword, setShowCurrentPassword] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [isUploadingAvatar, setIsUploadingAvatar] = useState(false);
  const [currentAvatar, setCurrentAvatar] = useState<string | null>(null);
  const [avatarUploadSuccess, setAvatarUploadSuccess] = useState(false);
  const [avatarUploadError, setAvatarUploadError] = useState<string | null>(null);
  const [sessions, setSessions] = useState<Session[]>([]);
  const [loadingSessions, setLoadingSessions] = useState(false);
  const [profileForm, setProfileForm] = useState<ProfileForm>({
    username: 'John Developer',
    email: 'john@example.com',
    firstName: 'John',
    lastName: 'Developer',
    bio: 'Full-stack developer passionate about CI/CD and automation.',
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });

  const tabs: Tab[] = [
    { id: 'profile', name: 'Profile', icon: UserIcon },
    { id: 'account', name: 'Account', icon: EnvelopeIcon },
    { id: 'security', name: 'Security', icon: ShieldCheckIcon },
    { id: 'notifications', name: 'Notifications', icon: BellIcon },
  ];

  const handleProfileChange = (key: keyof ProfileForm, value: string) => {
    setProfileForm(prev => ({
      ...prev,
      [key]: value
    }));
  };

  const convertFileToBase64 = (file: File): Promise<string> => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        const base64String = reader.result as string;
        resolve(base64String);
      };
      reader.onerror = (error) => reject(error);
    });
  };

  const handleAvatarUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    // Clear previous messages
    setAvatarUploadSuccess(false);
    setAvatarUploadError(null);

    // Validate file type
    if (!file.type.startsWith('image/')) {
      setAvatarUploadError('Please select an image file');
      return;
    }

    // Validate file size (max 2MB)
    if (file.size > 2 * 1024 * 1024) {
      setAvatarUploadError('File size must be less than 2MB');
      return;
    }

    try {
      setIsUploadingAvatar(true);
      
      // Convert file to base64
      const base64Avatar = await convertFileToBase64(file);
      
      // Update user avatar via API
      const updatedUser = await UserService.updateUser({ avatar: base64Avatar });
      
      // Update local state
      setCurrentAvatar(updatedUser.avatar || base64Avatar);
      setAvatarUploadSuccess(true);
      
      // Hide success message after 3 seconds
      setTimeout(() => {
        setAvatarUploadSuccess(false);
      }, 3000);
      
    } catch (error) {
      console.error('Error uploading avatar:', error);
      setAvatarUploadError('Failed to upload avatar. Please try again.');
      
      // Hide error message after 5 seconds
      setTimeout(() => {
        setAvatarUploadError(null);
      }, 5000);
    } finally {
      setIsUploadingAvatar(false);
    }
  };

  const saveProfile = () => {
    console.log('Save profile:', profileForm);
    // TODO: Implement actual save logic
  };

  const changePassword = () => {
    if (profileForm.newPassword !== profileForm.confirmPassword) {
      alert('New passwords do not match');
      return;
    }
    if (profileForm.newPassword.length < 6) {
      alert('Password must be at least 6 characters');
      return;
    }
    console.log('Change password');
    // TODO: Implement password change logic
    setProfileForm(prev => ({
      ...prev,
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    }));
  };

  const loadSessions = async () => {
    try {
      setLoadingSessions(true);
      const sessionData = await SessionService.getSessions();
      setSessions(sessionData);
    } catch (error) {
      console.error('Failed to load sessions:', error);
    } finally {
      setLoadingSessions(false);
    }
  };

  // Load sessions when security tab is active
  useEffect(() => {
    if (activeTab === 'security') {
      loadSessions();
    }
  }, [activeTab]);

  return (
    <div className="fade-in">
      {/* Page Header */}
      <div className="section-header">
        <h1 className="text-xl font-medium text-gray-900">Profile Settings</h1>
        <p className="text-sm text-gray-600">Manage your account information and preferences</p>
      </div>

      <div className="flex flex-col lg:flex-row gap-6">
        {/* Sidebar Navigation */}
        <div className="lg:w-64">
          <div className="card">
            <nav className="space-y-1">
              {tabs.map((tab) => {
                const Icon = tab.icon;
                return (
                  <button
                    key={tab.id}
                    onClick={() => setActiveTab(tab.id)}
                    className={`w-full flex items-center px-3 py-2 text-sm font-medium rounded-md transition-colors ${
                      activeTab === tab.id
                        ? 'bg-gray-100 text-gray-900'
                        : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                    }`}
                  >
                    <Icon className="h-4 w-4 mr-3" />
                    {tab.name}
                  </button>
                );
              })}
            </nav>
          </div>
        </div>

        {/* Main Content */}
        <div className="flex-1">
          <div className="card">
            {/* Profile Tab */}
            {activeTab === 'profile' && (
              <div className="space-y-8">
                <div>
                  <h2 className="text-xl font-semibold text-gray-900 mb-6">Profile Information</h2>
                </div>

                {/* Avatar Section */}
            <div>
              <h3 className="text-lg font-medium text-gray-900 mb-6">Profile Picture</h3>
              <div className="flex flex-col sm:flex-row items-center sm:items-start space-y-4 sm:space-y-0 sm:space-x-6">
                <div className="relative">
                  <div className="w-24 h-24 rounded-full bg-gray-200 flex items-center justify-center overflow-hidden">
                    {currentAvatar ? (
                      <img
                        src={currentAvatar}
                        alt="User avatar"
                        className="w-full h-full object-cover"
                      />
                    ) : (
                      <UserIcon className="w-12 h-12 text-gray-400" />
                    )}
                    {isUploadingAvatar && (
                      <div className="absolute inset-0 bg-black bg-opacity-50 flex items-center justify-center rounded-full">
                        <div className="animate-spin rounded-full h-6 w-6 border-b-2 border-white"></div>
                      </div>
                    )}
                  </div>
                  <label className={`absolute bottom-0 right-0 bg-blue-600 rounded-full p-2 cursor-pointer hover:bg-blue-700 transition-colors shadow-lg ${isUploadingAvatar ? 'opacity-50 cursor-not-allowed' : ''}`}>
                    <CameraIcon className="w-4 h-4 text-white" />
                    <input
                      type="file"
                      accept="image/*"
                      onChange={handleAvatarUpload}
                      disabled={isUploadingAvatar}
                      className="hidden"
                    />
                  </label>
                </div>
                <div className="text-center sm:text-left">
                  <p className="text-xs text-gray-500 mt-6">
                    Click the camera icon to change your profile picture<br/>
                    Recommended: Square image, at least 200×200px<br/>
                    JPG, GIF or PNG. Max size 2MB.
                  </p>
                  {isUploadingAvatar && (
                    <p className="text-xs text-blue-600 mt-2 font-medium">
                      Uploading avatar...
                    </p>
                  )}
                  {avatarUploadSuccess && (
                    <p className="text-xs text-green-600 mt-2 font-medium">
                      ✓ Avatar updated successfully!
                    </p>
                  )}
                  {avatarUploadError && (
                    <p className="text-xs text-red-600 mt-2 font-medium">
                      ⚠ {avatarUploadError}
                    </p>
                  )}
                </div>
              </div>
            </div>

            {/* Basic Information */}
            <div>
              <h3 className="text-lg font-medium text-gray-900 mb-4">Basic Information</h3>
              <div className="space-y-6">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      First Name
                    </label>
                    <input
                      type="text"
                      value={profileForm.firstName}
                      onChange={(e) => handleProfileChange('firstName', e.target.value)}
                      className="minimal-input"
                      placeholder="Enter first name"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Last Name
                    </label>
                    <input
                      type="text"
                      value={profileForm.lastName}
                      onChange={(e) => handleProfileChange('lastName', e.target.value)}
                      className="minimal-input"
                      placeholder="Enter last name"
                    />
                  </div>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Username
                  </label>
                  <input
                    type="text"
                    value={profileForm.username}
                    onChange={(e) => handleProfileChange('username', e.target.value)}
                    className="minimal-input"
                    placeholder="Enter username"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Email
                  </label>
                  <input
                    type="email"
                    value={profileForm.email}
                    onChange={(e) => handleProfileChange('email', e.target.value)}
                    className="minimal-input"
                    placeholder="Enter email"
                  />
                </div>
              </div>
            </div>

            {/* Bio Section */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Bio
              </label>
              <textarea
                value={profileForm.bio}
                onChange={(e) => handleProfileChange('bio', e.target.value)}
                rows={3}
                className="minimal-input resize-none"
                placeholder="Tell us about yourself..."
              />
              <p className="text-xs text-gray-500 mt-1">
                Brief description for your profile.
              </p>
            </div>

                {/* Save Button */}
                <div className="pt-8 border-t border-gray-200">
                  <div className="flex justify-end space-x-3">
                    <Link to="/" className="btn-secondary px-6">
                      Cancel
                    </Link>
                    <button onClick={saveProfile} className="btn-primary px-6">
                      Save Changes
                    </button>
                  </div>
                </div>
              </div>
            )}

            {/* Account Tab */}
            {activeTab === 'account' && (
              <div className="space-y-8">
                <div>
                  <h2 className="text-xl font-semibold text-gray-900 mb-6">Account Settings</h2>
                </div>

                {/* Email Settings */}
                <div>
                  <h3 className="text-lg font-medium text-gray-900 mb-4">Email Address</h3>
                  <div className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        Current Email
                      </label>
                      <div className="flex items-center space-x-3">
                        <input
                          type="email"
                          value={profileForm.email}
                          onChange={(e) => handleProfileChange('email', e.target.value)}
                          className="minimal-input"
                          placeholder="Enter email"
                        />
                        <span className="text-xs text-green-600 font-medium">Verified</span>
                      </div>
                    </div>
                    <button className="btn-secondary text-sm">
                      Change Email Address
                    </button>
                  </div>
                </div>

                {/* Language & Region */}
                <div className="pt-6 border-t border-gray-200">
                  <h3 className="text-lg font-medium text-gray-900 mb-4">Language & Region</h3>
                  <div className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        Language
                      </label>
                      <select className="minimal-input max-w-xs">
                        <option value="en">English</option>
                        <option value="zh">中文</option>
                        <option value="ja">日本語</option>
                      </select>
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        Timezone
                      </label>
                      <select className="minimal-input max-w-xs">
                        <option value="UTC">UTC</option>
                        <option value="Asia/Shanghai">Asia/Shanghai</option>
                        <option value="America/New_York">America/New York</option>
                      </select>
                    </div>
                  </div>
                </div>

                {/* Account Actions */}
                <div className="pt-6 border-t border-gray-200">
                  <h3 className="text-lg font-medium text-gray-900 mb-4">Account Actions</h3>
                  <div className="space-y-3">
                    <button className="btn-secondary w-full sm:w-auto">
                      Download Account Data
                    </button>
                    <button className="btn-secondary w-full sm:w-auto">
                      Deactivate Account
                    </button>
                  </div>
                </div>

                <div className="pt-6 border-t border-gray-200">
                  <div className="flex justify-end space-x-3">
                    <button className="btn-secondary px-6">Cancel</button>
                    <button className="btn-primary px-6">Save Changes</button>
                  </div>
                </div>
              </div>
            )}

            {/* Security Tab */}
            {activeTab === 'security' && (
              <div className="space-y-8">
                <div>
                  <h2 className="text-xl font-semibold text-gray-900 mb-6">Security Settings</h2>
                </div>

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
                          value={profileForm.currentPassword}
                          onChange={(e) => handleProfileChange('currentPassword', e.target.value)}
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
                          value={profileForm.newPassword}
                          onChange={(e) => handleProfileChange('newPassword', e.target.value)}
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
                          value={profileForm.confirmPassword}
                          onChange={(e) => handleProfileChange('confirmPassword', e.target.value)}
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
                      {profileForm.newPassword && profileForm.confirmPassword && 
                       profileForm.newPassword !== profileForm.confirmPassword && (
                        <p className="text-xs text-red-600 mt-1">Passwords do not match</p>
                      )}
                    </div>
                    <button
                      onClick={changePassword}
                      disabled={!profileForm.currentPassword || !profileForm.newPassword || 
                               profileForm.newPassword !== profileForm.confirmPassword}
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
                    <button className="btn-secondary text-sm">
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
                          const isCurrentDevice = SessionService.isCurrentSession(session);
                          const deviceType = SessionService.getDeviceTypeDisplay(session.deviceType);
                          const browserName = SessionService.getBrowserName(session.userAgent);
                          const lastActive = SessionService.formatLastActive(session.lastActiveAt);
                          
                          return (
                            <div key={session.sessionToken} className="flex items-center justify-between p-3 border border-gray-200 rounded-lg">
                              <div className="flex-1">
                                <div className="flex items-center space-x-2">
                                  <div className="text-sm font-medium text-gray-900">
                                    {browserName} on {deviceType}
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
                      <button className="btn-danger shrink-0">
                        Delete Account
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* Notifications Tab */}
            {activeTab === 'notifications' && (
              <div className="space-y-8">
                <div>
                  <h2 className="text-xl font-semibold text-gray-900 mb-6">Notification Settings</h2>
                </div>

                {/* Email Notifications */}
                <div>
                  <h3 className="text-lg font-medium text-gray-900 mb-4">Email Notifications</h3>
                  <div className="space-y-4">
                    <label className="flex items-center">
                      <input type="checkbox" defaultChecked className="mr-3" />
                      <div>
                        <div className="text-sm font-medium text-gray-900">Build notifications</div>
                        <div className="text-sm text-gray-500">Get notified when builds complete</div>
                      </div>
                    </label>
                    <label className="flex items-center">
                      <input type="checkbox" defaultChecked className="mr-3" />
                      <div>
                        <div className="text-sm font-medium text-gray-900">Failed builds</div>
                        <div className="text-sm text-gray-500">Only notify when builds fail</div>
                      </div>
                    </label>
                    <label className="flex items-center">
                      <input type="checkbox" className="mr-3" />
                      <div>
                        <div className="text-sm font-medium text-gray-900">Weekly summary</div>
                        <div className="text-sm text-gray-500">Weekly report of your build activity</div>
                      </div>
                    </label>
                  </div>
                </div>

                {/* Browser Notifications */}
                <div className="pt-6 border-t border-gray-200">
                  <h3 className="text-lg font-medium text-gray-900 mb-4">Browser Notifications</h3>
                  <div className="space-y-4">
                    <label className="flex items-center">
                      <input type="checkbox" defaultChecked className="mr-3" />
                      <div>
                        <div className="text-sm font-medium text-gray-900">Desktop notifications</div>
                        <div className="text-sm text-gray-500">Show notifications in your browser</div>
                      </div>
                    </label>
                    <label className="flex items-center">
                      <input type="checkbox" className="mr-3" />
                      <div>
                        <div className="text-sm font-medium text-gray-900">Sound notifications</div>
                        <div className="text-sm text-gray-500">Play sound when receiving notifications</div>
                      </div>
                    </label>
                  </div>
                </div>

                {/* Mobile Notifications */}
                <div className="pt-6 border-t border-gray-200">
                  <h3 className="text-lg font-medium text-gray-900 mb-4">Mobile Notifications</h3>
                  <div className="space-y-4">
                    <label className="flex items-center">
                      <input type="checkbox" className="mr-3" />
                      <div>
                        <div className="text-sm font-medium text-gray-900">Push notifications</div>
                        <div className="text-sm text-gray-500">Receive push notifications on mobile devices</div>
                      </div>
                    </label>
                  </div>
                </div>

                <div className="pt-6 border-t border-gray-200">
                  <div className="flex justify-end space-x-3">
                    <button className="btn-secondary px-6">Cancel</button>
                    <button className="btn-primary px-6">Save Changes</button>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default UserProfile;