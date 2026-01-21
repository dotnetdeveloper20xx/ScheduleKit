import { useState } from 'react';
import { Button, Input } from '@/components/ui';

interface EmbedCodeModalProps {
  isOpen: boolean;
  onClose: () => void;
  hostSlug: string;
  eventSlug: string;
}

export function EmbedCodeModal({ isOpen, onClose, hostSlug, eventSlug }: EmbedCodeModalProps) {
  const [primaryColor, setPrimaryColor] = useState('#3b82f6');
  const [width, setWidth] = useState('100%');
  const [height, setHeight] = useState('500');
  const [copied, setCopied] = useState(false);

  if (!isOpen) return null;

  const baseUrl = window.location.origin;
  const widgetUrl = `${baseUrl}/widget/${hostSlug}/${eventSlug}?primaryColor=${encodeURIComponent(primaryColor)}`;

  const embedCode = `<!-- ScheduleKit Booking Widget -->
<iframe
  src="${widgetUrl}"
  width="${width}"
  height="${height}px"
  frameborder="0"
  style="border: 1px solid #e5e7eb; border-radius: 8px;"
></iframe>

<!-- Optional: Auto-resize script -->
<script>
  window.addEventListener('message', function(event) {
    if (event.data && event.data.type === 'schedulekit-resize') {
      var iframe = document.querySelector('iframe[src*="${hostSlug}/${eventSlug}"]');
      if (iframe) {
        iframe.style.height = event.data.height + 'px';
      }
    }
    if (event.data && event.data.type === 'schedulekit-booking-complete') {
      console.log('Booking completed:', event.data.booking);
      // Add your custom booking completion handler here
    }
  });
</script>`;

  const handleCopy = () => {
    navigator.clipboard.writeText(embedCode);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div className="mx-4 w-full max-w-2xl rounded-lg bg-white shadow-xl">
        {/* Header */}
        <div className="flex items-center justify-between border-b px-6 py-4">
          <h2 className="text-lg font-semibold text-gray-900">Embed Booking Widget</h2>
          <button
            onClick={onClose}
            className="rounded-lg p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600"
          >
            <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* Content */}
        <div className="p-6">
          {/* Customization Options */}
          <div className="mb-6 grid grid-cols-3 gap-4">
            <div>
              <label className="mb-1 block text-sm font-medium text-gray-700">
                Primary Color
              </label>
              <div className="flex items-center gap-2">
                <input
                  type="color"
                  value={primaryColor}
                  onChange={(e) => setPrimaryColor(e.target.value)}
                  className="h-9 w-12 cursor-pointer rounded border border-gray-300"
                />
                <Input
                  value={primaryColor}
                  onChange={(e) => setPrimaryColor(e.target.value)}
                  className="flex-1 font-mono text-sm"
                />
              </div>
            </div>
            <div>
              <label className="mb-1 block text-sm font-medium text-gray-700">
                Width
              </label>
              <Input
                value={width}
                onChange={(e) => setWidth(e.target.value)}
                placeholder="100% or 400px"
              />
            </div>
            <div>
              <label className="mb-1 block text-sm font-medium text-gray-700">
                Height (px)
              </label>
              <Input
                type="number"
                value={height}
                onChange={(e) => setHeight(e.target.value)}
                min="300"
              />
            </div>
          </div>

          {/* Preview */}
          <div className="mb-6">
            <label className="mb-2 block text-sm font-medium text-gray-700">Preview</label>
            <div className="rounded-lg border bg-gray-50 p-4">
              <iframe
                src={widgetUrl}
                width={width}
                height={`${height}px`}
                frameBorder="0"
                className="rounded-lg border border-gray-200"
                title="Widget Preview"
              />
            </div>
          </div>

          {/* Embed Code */}
          <div>
            <div className="mb-2 flex items-center justify-between">
              <label className="text-sm font-medium text-gray-700">Embed Code</label>
              <Button variant="outline" size="sm" onClick={handleCopy}>
                {copied ? (
                  <>
                    <svg className="mr-1 h-4 w-4 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                    </svg>
                    Copied!
                  </>
                ) : (
                  <>
                    <svg className="mr-1 h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                    </svg>
                    Copy Code
                  </>
                )}
              </Button>
            </div>
            <pre className="max-h-48 overflow-auto rounded-lg bg-gray-900 p-4 text-xs text-gray-100">
              <code>{embedCode}</code>
            </pre>
          </div>

          {/* Direct Link */}
          <div className="mt-4 rounded-lg bg-blue-50 p-4">
            <p className="mb-2 text-sm font-medium text-blue-900">Direct Link</p>
            <div className="flex items-center gap-2">
              <code className="flex-1 truncate rounded bg-white px-3 py-2 text-sm text-blue-800">
                {baseUrl}/book/{hostSlug}/{eventSlug}
              </code>
              <Button
                variant="outline"
                size="sm"
                onClick={() => {
                  navigator.clipboard.writeText(`${baseUrl}/book/${hostSlug}/${eventSlug}`);
                }}
              >
                Copy
              </Button>
            </div>
          </div>
        </div>

        {/* Footer */}
        <div className="flex justify-end border-t px-6 py-4">
          <Button variant="outline" onClick={onClose}>
            Close
          </Button>
        </div>
      </div>
    </div>
  );
}
