# AppinstallerWorkaround

This is a workaround for the issue here: https://techcommunity.microsoft.com/t5/msix-deployment/windows-10-2004-msix-not-updating-please-check-whether-the/m-p/1466701

Usage:
```
   // App.xaml
    protected override async void OnLaunched(LaunchActivatedEventArgs e) {
        if (setup_kit_util.has_update_downloaded_locally() && await setup_kit_util.launch_update_kit()) {
            Application.Current.Exit();
            return;
        }
        // note: you can place this anywhere, as long as it's AFTER the above code
        Task.Run(async () => await setup_kit_util.download_update_task());

        ...
    }
```

Please make sure to look at setup_kit_util, and update the following: 
- updates_dir
- appinstaller_url
- msix_url
