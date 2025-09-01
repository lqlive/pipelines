import React, { useState } from 'react';
import {
  UserIcon,
  CameraIcon,
} from '@heroicons/react/24/outline';
import { UserService } from '../../services/userService';

interface ProfileForm {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  bio: string;
}

interface ProfileTabProps {
  onNotification?: (type: 'success' | 'error', message: string) => void;
}

const ProfileTab: React.FC<ProfileTabProps> = ({ onNotification }) => {
  const [isUploadingAvatar, setIsUploadingAvatar] = useState(false);
  const [currentAvatar, setCurrentAvatar] = useState<string | null>(null);
  const [avatarUploadSuccess, setAvatarUploadSuccess] = useState(false);
  const [avatarUploadError, setAvatarUploadError] = useState<string | null>(null);

  const [profileForm, setProfileForm] = useState<ProfileForm>({
    username: 'John Developer',
    email: 'john@example.com',
    firstName: 'John',
    lastName: 'Developer',
    bio: 'Full-stack developer passionate about CI/CD and automation.',
  });

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
      showNotification('error', 'Please select an image file');
      return;
    }

    // Validate file size (max 2MB)
    if (file.size > 2 * 1024 * 1024) {
      showNotification('error', 'File size must be less than 2MB');
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
    if (onNotification) {
      onNotification('success', 'Profile updated successfully');
    }
  };

  const showNotification = (type: 'success' | 'error', message: string) => {
    if (onNotification) {
      onNotification(type, message);
    }
  };

  return (
    <div className="space-y-8">
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
              <button className="btn-secondary px-6">
                Cancel
              </button>
              <button onClick={saveProfile} className="btn-primary px-6">
                Save Changes
              </button>
            </div>
          </div>
    </div>
  );
};

export default ProfileTab;
