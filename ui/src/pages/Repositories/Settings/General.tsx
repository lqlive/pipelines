import React from 'react';

interface GeneralSettings {
  description: string;
  website: string;
  topics: string[];
  visibility: 'public' | 'private';
  defaultBranch: string;
}

interface GeneralProps {
  settings: GeneralSettings;
  onSettingChange: (key: keyof GeneralSettings, value: any) => void;
}

const General: React.FC<GeneralProps> = ({ settings, onSettingChange }) => {
  return (
    <div>
      <h2 className="text-xl font-semibold text-gray-900 mb-6">General Settings</h2>
      <div className="space-y-6">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Repository Description
          </label>
          <textarea
            value={settings.description}
            onChange={(e) => onSettingChange('description', e.target.value)}
            className="minimal-input"
            rows={3}
            placeholder="A short description of your repository"
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Website URL
          </label>
          <input
            type="url"
            value={settings.website}
            onChange={(e) => onSettingChange('website', e.target.value)}
            className="minimal-input max-w-md"
            placeholder="https://example.com"
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Topics
          </label>
          <input
            type="text"
            value={settings.topics.join(', ')}
            onChange={(e) => onSettingChange('topics', e.target.value.split(', ').filter(t => t.trim()))}
            className="minimal-input max-w-md"
            placeholder="api, backend, nodejs"
          />
          <p className="mt-1 text-sm text-gray-500">Separate topics with commas</p>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Visibility
          </label>
          <div className="space-y-2">
            <label className="flex items-center">
              <input
                type="radio"
                name="visibility"
                value="public"
                checked={settings.visibility === 'public'}
                onChange={(e) => onSettingChange('visibility', e.target.value as 'public' | 'private')}
                className="mr-2"
              />
              <div>
                <div className="text-sm font-medium">Public</div>
                <div className="text-sm text-gray-500">Anyone can see this repository</div>
              </div>
            </label>
            <label className="flex items-center">
              <input
                type="radio"
                name="visibility"
                value="private"
                checked={settings.visibility === 'private'}
                onChange={(e) => onSettingChange('visibility', e.target.value as 'public' | 'private')}
                className="mr-2"
              />
              <div>
                <div className="text-sm font-medium">Private</div>
                <div className="text-sm text-gray-500">Only you and collaborators can see this repository</div>
              </div>
            </label>
          </div>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Default Branch
          </label>
          <select
            value={settings.defaultBranch}
            onChange={(e) => onSettingChange('defaultBranch', e.target.value)}
            className="minimal-input max-w-xs"
          >
            <option value="main">main</option>
            <option value="master">master</option>
            <option value="develop">develop</option>
          </select>
        </div>
      </div>
    </div>
  );
};

export default General;
