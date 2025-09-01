import React from 'react';

interface BuildsSettings {
  enabled: boolean;
  autoCancel: boolean;
  timeout: number;
  concurrent: number;
  protected: boolean;
}

interface BuildsProps {
  settings: BuildsSettings;
  onSettingChange: (key: keyof BuildsSettings, value: any) => void;
}

const Builds: React.FC<BuildsProps> = ({ settings, onSettingChange }) => {
  return (
    <div>
      <h2 className="text-xl font-semibold text-gray-900 mb-6">Build Settings</h2>
      <div className="space-y-6">
        <div>
          <label className="flex items-center justify-between">
            <div>
              <div className="text-sm font-medium text-gray-900">Enable builds</div>
              <div className="text-sm text-gray-500">Allow this repository to trigger builds</div>
            </div>
            <input
              type="checkbox"
              checked={settings.enabled}
              onChange={(e) => onSettingChange('enabled', e.target.checked)}
              className="ml-3"
            />
          </label>
        </div>

        <div>
          <label className="flex items-center justify-between">
            <div>
              <div className="text-sm font-medium text-gray-900">Auto-cancel redundant builds</div>
              <div className="text-sm text-gray-500">Cancel previous builds when new commits are pushed</div>
            </div>
            <input
              type="checkbox"
              checked={settings.autoCancel}
              onChange={(e) => onSettingChange('autoCancel', e.target.checked)}
              className="ml-3"
            />
          </label>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Build timeout (minutes)
          </label>
          <input
            type="number"
            value={settings.timeout}
            onChange={(e) => onSettingChange('timeout', parseInt(e.target.value))}
            className="minimal-input max-w-xs"
            min="1"
            max="600"
          />
          <p className="mt-1 text-sm text-gray-500">Maximum time a build can run before being cancelled</p>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Concurrent builds
          </label>
          <input
            type="number"
            value={settings.concurrent}
            onChange={(e) => onSettingChange('concurrent', parseInt(e.target.value))}
            className="minimal-input max-w-xs"
            min="1"
            max="10"
          />
          <p className="mt-1 text-sm text-gray-500">Maximum number of builds that can run simultaneously</p>
        </div>

        <div>
          <label className="flex items-center justify-between">
            <div>
              <div className="text-sm font-medium text-gray-900">Protected builds</div>
              <div className="text-sm text-gray-500">Require status checks to pass before merging</div>
            </div>
            <input
              type="checkbox"
              checked={settings.protected}
              onChange={(e) => onSettingChange('protected', e.target.checked)}
              className="ml-3"
            />
          </label>
        </div>
      </div>
    </div>
  );
};

export default Builds;
