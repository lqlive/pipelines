import React, { useState } from 'react';

interface NotificationForm {
  emailNotifications: {
    buildNotifications: boolean;
    failedBuilds: boolean;
    weeklySummary: boolean;
  };
  browserNotifications: {
    desktopNotifications: boolean;
    soundNotifications: boolean;
  };
  mobileNotifications: {
    pushNotifications: boolean;
  };
}

interface NotificationTabProps {
  onNotification?: (type: 'success' | 'error', message: string) => void;
}

const NotificationTab: React.FC<NotificationTabProps> = ({ onNotification }) => {
  const [notificationForm, setNotificationForm] = useState<NotificationForm>({
    emailNotifications: {
      buildNotifications: true,
      failedBuilds: true,
      weeklySummary: false,
    },
    browserNotifications: {
      desktopNotifications: true,
      soundNotifications: false,
    },
    mobileNotifications: {
      pushNotifications: false,
    },
  });

  const handleEmailNotificationChange = (key: keyof NotificationForm['emailNotifications'], value: boolean) => {
    setNotificationForm(prev => ({
      ...prev,
      emailNotifications: {
        ...prev.emailNotifications,
        [key]: value
      }
    }));
  };

  const handleBrowserNotificationChange = (key: keyof NotificationForm['browserNotifications'], value: boolean) => {
    setNotificationForm(prev => ({
      ...prev,
      browserNotifications: {
        ...prev.browserNotifications,
        [key]: value
      }
    }));
  };

  const handleMobileNotificationChange = (key: keyof NotificationForm['mobileNotifications'], value: boolean) => {
    setNotificationForm(prev => ({
      ...prev,
      mobileNotifications: {
        ...prev.mobileNotifications,
        [key]: value
      }
    }));
  };

  const saveNotificationSettings = () => {
    console.log('Save notification settings:', notificationForm);
    // TODO: Implement actual save logic
    if (onNotification) {
      onNotification('success', 'Notification settings updated successfully');
    }
  };

  return (
    <div className="space-y-8">
          {/* Email Notifications */}
          <div>
            <h3 className="text-lg font-medium text-gray-900 mb-4">Email Notifications</h3>
            <div className="space-y-4">
              <label className="flex items-center">
                <input 
                  type="checkbox" 
                  checked={notificationForm.emailNotifications.buildNotifications}
                  onChange={(e) => handleEmailNotificationChange('buildNotifications', e.target.checked)}
                  className="mr-3" 
                />
                <div>
                  <div className="text-sm font-medium text-gray-900">Build notifications</div>
                  <div className="text-sm text-gray-500">Get notified when builds complete</div>
                </div>
              </label>
              <label className="flex items-center">
                <input 
                  type="checkbox" 
                  checked={notificationForm.emailNotifications.failedBuilds}
                  onChange={(e) => handleEmailNotificationChange('failedBuilds', e.target.checked)}
                  className="mr-3" 
                />
                <div>
                  <div className="text-sm font-medium text-gray-900">Failed builds</div>
                  <div className="text-sm text-gray-500">Only notify when builds fail</div>
                </div>
              </label>
              <label className="flex items-center">
                <input 
                  type="checkbox" 
                  checked={notificationForm.emailNotifications.weeklySummary}
                  onChange={(e) => handleEmailNotificationChange('weeklySummary', e.target.checked)}
                  className="mr-3" 
                />
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
                <input 
                  type="checkbox" 
                  checked={notificationForm.browserNotifications.desktopNotifications}
                  onChange={(e) => handleBrowserNotificationChange('desktopNotifications', e.target.checked)}
                  className="mr-3" 
                />
                <div>
                  <div className="text-sm font-medium text-gray-900">Desktop notifications</div>
                  <div className="text-sm text-gray-500">Show notifications in your browser</div>
                </div>
              </label>
              <label className="flex items-center">
                <input 
                  type="checkbox" 
                  checked={notificationForm.browserNotifications.soundNotifications}
                  onChange={(e) => handleBrowserNotificationChange('soundNotifications', e.target.checked)}
                  className="mr-3" 
                />
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
                <input 
                  type="checkbox" 
                  checked={notificationForm.mobileNotifications.pushNotifications}
                  onChange={(e) => handleMobileNotificationChange('pushNotifications', e.target.checked)}
                  className="mr-3" 
                />
                <div>
                  <div className="text-sm font-medium text-gray-900">Push notifications</div>
                  <div className="text-sm text-gray-500">Receive push notifications on mobile devices</div>
                </div>
              </label>
            </div>
          </div>

          {/* Save Button */}
          <div className="pt-6 border-t border-gray-200">
            <div className="flex justify-end space-x-3">
              <button className="btn-secondary px-6">Cancel</button>
              <button onClick={saveNotificationSettings} className="btn-primary px-6">Save Changes</button>
            </div>
          </div>
    </div>
  );
};

export default NotificationTab;
