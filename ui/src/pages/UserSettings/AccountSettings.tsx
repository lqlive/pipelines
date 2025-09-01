import React, { useState } from 'react';

interface AccountForm {
  email: string;
  language: string;
  timezone: string;
}

interface AccountTabProps {
  onNotification?: (type: 'success' | 'error', message: string) => void;
}

const AccountTab: React.FC<AccountTabProps> = ({ onNotification }) => {
  const [accountForm, setAccountForm] = useState<AccountForm>({
    email: 'john@example.com',
    language: 'en',
    timezone: 'UTC',
  });

  const handleAccountChange = (key: keyof AccountForm, value: string) => {
    setAccountForm(prev => ({
      ...prev,
      [key]: value
    }));
  };

  const saveAccountSettings = () => {
    console.log('Save account settings:', accountForm);
    // TODO: Implement actual save logic
    if (onNotification) {
      onNotification('success', 'Account settings updated successfully');
    }
  };

  const changeEmail = () => {
    console.log('Change email');
    // TODO: Implement email change logic
    if (onNotification) {
      onNotification('success', 'Email change request sent. Please check your inbox.');
    }
  };

  const downloadAccountData = () => {
    console.log('Download account data');
    // TODO: Implement account data download
    if (onNotification) {
      onNotification('success', 'Account data download started');
    }
  };

  const deactivateAccount = () => {
    console.log('Deactivate account');
    // TODO: Implement account deactivation
    if (confirm('Are you sure you want to deactivate your account? This action can be reversed within 30 days.')) {
      if (onNotification) {
        onNotification('success', 'Account deactivation initiated');
      }
    }
  };

  return (
    <div className="space-y-8">
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
                    value={accountForm.email}
                    onChange={(e) => handleAccountChange('email', e.target.value)}
                    className="minimal-input"
                    placeholder="Enter email"
                  />
                  <span className="text-xs text-green-600 font-medium">Verified</span>
                </div>
              </div>
              <button onClick={changeEmail} className="btn-secondary text-sm">
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
                <select 
                  value={accountForm.language}
                  onChange={(e) => handleAccountChange('language', e.target.value)}
                  className="minimal-input max-w-xs"
                >
                  <option value="en">English</option>
                  <option value="zh">中文</option>
                  <option value="ja">日本語</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Timezone
                </label>
                <select 
                  value={accountForm.timezone}
                  onChange={(e) => handleAccountChange('timezone', e.target.value)}
                  className="minimal-input max-w-xs"
                >
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
              <button onClick={downloadAccountData} className="btn-secondary w-full sm:w-auto">
                Download Account Data
              </button>
              <button onClick={deactivateAccount} className="btn-secondary w-full sm:w-auto">
                Deactivate Account
              </button>
            </div>
          </div>

          {/* Save Button */}
          <div className="pt-6 border-t border-gray-200">
            <div className="flex justify-end space-x-3">
              <button className="btn-secondary px-6">Cancel</button>
              <button onClick={saveAccountSettings} className="btn-primary px-6">Save Changes</button>
            </div>
          </div>
    </div>
  );
};

export default AccountTab;
