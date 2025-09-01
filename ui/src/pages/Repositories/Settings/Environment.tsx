import React from 'react';
import { TrashIcon } from '@heroicons/react/24/outline';

interface EnvironmentVariable {
  key: string;
  value: string;
  secret: boolean;
}

interface EnvironmentSettings {
  variables: EnvironmentVariable[];
}

interface EnvironmentProps {
  settings: EnvironmentSettings;
  onSettingChange: (key: keyof EnvironmentSettings, value: any) => void;
}

const Environment: React.FC<EnvironmentProps> = ({ settings, onSettingChange }) => {
  return (
    <div>
      <h2 className="text-xl font-semibold text-gray-900 mb-6">Environment Variables</h2>
      <div className="space-y-4">
        {settings.variables.map((variable, index) => (
          <div key={index} className="flex items-center space-x-3 p-3 border border-gray-200 rounded-md">
            <input
              type="text"
              value={variable.key}
              onChange={(e) => {
                const newVars = [...settings.variables];
                newVars[index].key = e.target.value;
                onSettingChange('variables', newVars);
              }}
              className="minimal-input flex-1"
              placeholder="Variable name"
            />
            <input
              type={variable.secret ? "password" : "text"}
              value={variable.value}
              onChange={(e) => {
                const newVars = [...settings.variables];
                newVars[index].value = e.target.value;
                onSettingChange('variables', newVars);
              }}
              className="minimal-input flex-1"
              placeholder="Variable value"
            />
            <label className="flex items-center">
              <input
                type="checkbox"
                checked={variable.secret}
                onChange={(e) => {
                  const newVars = [...settings.variables];
                  newVars[index].secret = e.target.checked;
                  onSettingChange('variables', newVars);
                }}
                className="mr-1"
              />
              <span className="text-xs">Secret</span>
            </label>
            <button
              onClick={() => {
                const newVars = settings.variables.filter((_, i) => i !== index);
                onSettingChange('variables', newVars);
              }}
              className="text-red-600 hover:text-red-800"
            >
              <TrashIcon className="h-4 w-4" />
            </button>
          </div>
        ))}
        
        <button
          onClick={() => {
            const newVars = [...settings.variables, { key: '', value: '', secret: false }];
            onSettingChange('variables', newVars);
          }}
          className="btn-secondary"
        >
          Add Variable
        </button>
      </div>
    </div>
  );
};

export default Environment;
