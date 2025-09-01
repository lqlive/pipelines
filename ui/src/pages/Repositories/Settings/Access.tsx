import React from 'react';

interface AccessSettings {
  adminUsers: string[];
  writeUsers: string[];
  readUsers: string[];
}

interface AccessProps {
  settings: AccessSettings;
  onSettingChange: (key: keyof AccessSettings, value: any) => void;
}

const Access: React.FC<AccessProps> = ({ settings, onSettingChange }) => {
  return (
    <div>
      <h2 className="text-xl font-semibold text-gray-900 mb-6">Access Control</h2>
      <div className="space-y-6">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Admin Users
          </label>
          <textarea
            value={settings.adminUsers.join('\n')}
            onChange={(e) => onSettingChange('adminUsers', e.target.value.split('\n').filter(u => u.trim()))}
            className="minimal-input"
            rows={3}
            placeholder="admin@example.com&#10;admin2@example.com"
          />
          <p className="mt-1 text-sm text-gray-500">One email per line. Admin users can modify settings</p>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Write Users
          </label>
          <textarea
            value={settings.writeUsers.join('\n')}
            onChange={(e) => onSettingChange('writeUsers', e.target.value.split('\n').filter(u => u.trim()))}
            className="minimal-input"
            rows={3}
            placeholder="dev@example.com&#10;dev2@example.com"
          />
          <p className="mt-1 text-sm text-gray-500">Write users can trigger builds and view logs</p>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Read Users
          </label>
          <textarea
            value={settings.readUsers.join('\n')}
            onChange={(e) => onSettingChange('readUsers', e.target.value.split('\n').filter(u => u.trim()))}
            className="minimal-input"
            rows={3}
            placeholder="readonly@example.com&#10;guest@example.com"
          />
          <p className="mt-1 text-sm text-gray-500">Read users can only view build status and logs</p>
        </div>
      </div>
    </div>
  );
};

export default Access;
