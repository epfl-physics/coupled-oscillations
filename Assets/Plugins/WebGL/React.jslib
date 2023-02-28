mergeInto(LibraryManager.library, {
  EnterFullscreen: function () {
    try {
      window.dispatchReactUnityEvent('EnterFullscreen');
    } catch (e) {
      console.warn('Failed to dispatch event');
    }
  },

  ExitFullscreen: function () {
    try {
      window.dispatchReactUnityEvent('ExitFullscreen');
    } catch (e) {
      console.warn('Failed to dispatch event');
    }
  },
});
