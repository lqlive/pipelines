import React from 'react';

interface WebhooksSettings {
  url: string;
  events: string[];
  secret: string;
  enabled: boolean;
}

interface WebhooksProps {
  settings: WebhooksSettings;
  onSettingChange: (key: keyof WebhooksSettings, value: any) => void;
}

const Webhooks: React.FC<WebhooksProps> = ({ settings, onSettingChange }) => {
  return (
    <div>
      <h2 className="text-xl font-semibold text-gray-900 mb-6">Webhook Settings</h2>
      <div className="space-y-6">
        <div>
          <label className="flex items-center justify-between mb-4">
            <div>
              <div className="text-sm font-medium text-gray-900">Enable webhooks</div>
              <div className="text-sm text-gray-500">Send HTTP requests when events occur</div>
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
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Payload URL
          </label>
          <input
            type="url"
            value={settings.url}
            onChange={(e) => onSettingChange('url', e.target.value)}
            className="minimal-input"
            placeholder="https://api.example.com/webhooks"
            disabled={!settings.enabled}
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Secret Token
          </label>
          <input
            type="password"
            value={settings.secret}
            onChange={(e) => onSettingChange('secret', e.target.value)}
            className="minimal-input max-w-md"
            placeholder="Optional secret for payload validation"
            disabled={!settings.enabled}
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Events
          </label>
          <div className="space-y-2">
            {['push', 'pull_request', 'tag', 'release'].map((event) => (
              <label key={event} className="flex items-center">
                <input
                  type="checkbox"
                  checked={settings.events.includes(event)}
                  onChange={(e) => {
                    const events = e.target.checked
                      ? [...settings.events, event]
                      : settings.events.filter(e => e !== event);
                    onSettingChange('events', events);
                  }}
                  className="mr-2"
                  disabled={!settings.enabled}
                />
                <span className="text-sm capitalize">{event.replace('_', ' ')}</span>
              </label>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Webhooks;
