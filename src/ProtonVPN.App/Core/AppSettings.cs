﻿/*
 * Copyright (c) 2020 Proton Technologies AG
 *
 * This file is part of ProtonVPN.
 *
 * ProtonVPN is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * ProtonVPN is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with ProtonVPN.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ProtonVPN.Common;
using ProtonVPN.Core.Announcements;
using ProtonVPN.Core.Auth;
using ProtonVPN.Core.Models;
using ProtonVPN.Core.Native.Structures;
using ProtonVPN.Core.Profiles.Cached;
using ProtonVPN.Core.Settings;
using ProtonVPN.Core.Settings.Contracts;
using ProtonVPN.Core.Storage;
using ProtonVPN.Settings;

namespace ProtonVPN.Core
{
    internal class AppSettings : IAppSettings, INotifyPropertyChanged, ILoggedInAware
    {
        private readonly ISettingsStorage _storage;
        private readonly UserSettings _userSettings;
        private readonly Common.Configuration.Config _config;
        private readonly HashSet<string> _accessedPerUserProperties = new HashSet<string>();

        public AppSettings(ISettingsStorage storage, UserSettings userSettings, Common.Configuration.Config config)
        {
            _config = config;
            _storage = storage;
            _userSettings = userSettings;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CachedProfileDataContract Profiles
        {
            get => GetPerUser<CachedProfileDataContract>() ?? new CachedProfileDataContract();
            set => SetPerUser(value);
        }

        public IReadOnlyList<AnnouncementItem> Announcements
        {
            get => GetPerUser<IReadOnlyList<AnnouncementItem>>() ?? new List<AnnouncementItem>();
            set => SetPerUser(value);
        }

        public DateTime ProfileChangesSyncedAt
        {
            get => GetPerUser<DateTime>();
            set => SetPerUser(value);
        }

        public string OvpnProtocol
        {
            get => Get<string>();
            set => Set(value);
        }

        public bool AppFirstRun
        {
            get => Get<bool>();
            set => Set(value);
        }

        public bool ShowNotifications
        {
            get => Get<bool>();
            set => Set(value);
        }

        public WindowPlacement WindowPlacement
        {
            get => Get<WindowPlacement>();
            set => Set(value);
        }

        public WindowPlacement SidebarWindowPlacement
        {
            get => Get<WindowPlacement>();
            set => Set(value);
        }

        public double Width
        {
            get => Get<double>();
            set => Set(value);
        }

        public string AutoConnect
        {
            get => GetPerUser<string>();
            set => SetPerUser(value);
        }

        public string QuickConnect
        {
            get => GetPerUser<string>();
            set => SetPerUser(value);
        }

        public bool StartOnStartup
        {
            get => Get<bool>();
            set => Set(value);
        }

        public StartMinimizedMode StartMinimized
        {
            get => Get<StartMinimizedMode>();
            set => Set(value);
        }

        public bool EarlyAccess
        {
            get => Get<bool>();
            set => Set(value);
        }

        public bool SecureCore
        {
            get => GetPerUser<bool>();
            set => SetPerUser(value);
        }

        public string LastUpdate
        {
            get => Get<string>();
            set => Set(value);
        }

        public string Language
        {
            get => Get<string>();
            set => Set(value);
        }

        public bool KillSwitch
        {
            get => Get<bool>();
            set => Set(value);
        }

        public bool Ipv6LeakProtection
        {
            get => Get<bool>();
            set => Set(value);
        }

        public bool CustomDnsEnabled
        {
            get => Get<bool>();
            set => Set(value);
        }

        public bool NetShieldEnabled
        {
            get => Get<bool>();
            set => Set(value);
        }

        public int NetShieldMode
        {
            get => Get<int>();
            set => Set(value);
        }

        public bool SidebarMode
        {
            get => Get<bool>();
            set => Set(value);
        }

        public bool WelcomeModalShown
        {
            get => GetPerUser<bool>();
            set => SetPerUser(value);
        }

        public long TrialExpirationTime
        {
            get => GetPerUser<long>();
            set => SetPerUser(value);
        }

        public bool AboutToExpireModalShown
        {
            get => GetPerUser<bool>();
            set => SetPerUser(value);
        }

        public bool NetShieldModalShown
        {
            get => GetPerUser<bool>();
            set => SetPerUser(value);
        }

        public bool ExpiredModalShown
        {
            get => GetPerUser<bool>();
            set => SetPerUser(value);
        }

        public int OnboardingStep
        {
            get => GetPerUser<int>();
            set => SetPerUser(value);
        }

        public string LastEventId
        {
            get => GetPerUser<string>();
            set => SetPerUser(value);
        }

        public int AppStartCounter
        {
            get => Get<int>();
            set => Set(value);
        }

        public int SidebarTab
        {
            get => Get<int>();
            set => Set(value);
        }

        public DateTime LastPrimaryApiFail
        {
            get => Get<DateTime>();
            set => Set(value);
        }

        public StringCollection AlternativeApiBaseUrls
        {
            get => Get<StringCollection>();
            set => Set(value);
        }

        public string ActiveAlternativeApiBaseUrl
        {
            get => Get<string>();
            set => Set(value);
        }

        public bool DoHEnabled
        {
            get => Get<bool>();
            set => Set(value);
        }

        public SplitTunnelingApp[] SplitTunnelingBlockApps
        {
            get => Get<SplitTunnelingApp[]>() ?? new SplitTunnelingApp[0];
            set => Set(value);
        }

        public SplitTunnelingApp[] SplitTunnelingAllowApps
        {
            get => Get<SplitTunnelingApp[]>() ?? new SplitTunnelingApp[0];
            set => Set(value);
        }

        public IpContract[] SplitTunnelExcludeIps
        {
            get => Get<IpContract[]>() ?? new IpContract[0];
            set => Set(value);
        }

        public IpContract[] SplitTunnelIncludeIps
        {
            get => Get<IpContract[]>() ?? new IpContract[0];
            set => Set(value);
        }

        public IpContract[] CustomDnsIps
        {
            get => Get<IpContract[]>() ?? new IpContract[0];
            set => Set(value);
        }

        public int SettingsSelectedTabIndex
        {
            get => Get<int>();
            set => Set(value);
        }

        public bool SplitTunnelingEnabled
        {
            get => Get<bool>();
            set => Set(value);
        }

        public SplitTunnelMode SplitTunnelMode
        {
            get => Get<SplitTunnelMode>();
            set => Set(value);
        }

        public int[] OpenVpnTcpPorts
        {
            get => Get<int[]>() ?? _config.DefaultOpenVpnTcpPorts;
            set => Set(value);
        }

        public int[] OpenVpnUdpPorts
        {
            get => Get<int[]>() ?? _config.DefaultOpenVpnUdpPorts;
            set => Set(value);
        }

        public StringCollection BlackHoleIps
        {
            get
            {
                var list = Get<StringCollection>();
                if (list == null)
                {
                    list = new StringCollection();
                    list.AddRange(_config.DefaultBlackHoleIps.ToArray());
                }

                return list;
            }

            set => Set(value);
        }

        public bool FeatureNetShieldEnabled
        {
            get => Get<bool>();
            set => Set(value);
        }

        public bool FeatureMaintenanceTrackerEnabled
        {
            get => Get<bool>();
            set => Set(value);
        }

        public bool FeaturePollNotificationApiEnabled
        {
            get => Get<bool>();
            set => Set(value);
        }

        public TimeSpan MaintenanceCheckInterval
        {
            get
            {
                var value = Get<TimeSpan>();
                if (value == TimeSpan.Zero)
                {
                    value = _config.MaintenanceCheckInterval;
                }

                return value;
            }
            set => Set(value);
        }

        public void OnUserLoggedIn()
        {
            var properties = _accessedPerUserProperties.ToList();
            _accessedPerUserProperties.Clear();
            OnPropertiesChanged(properties);
        }

        public string[] GetSplitTunnelApps()
        {
            string[] apps = { };

            switch (SplitTunnelMode)
            {
                case SplitTunnelMode.Block:
                    apps = GetApps(SplitTunnelingBlockApps);
                    break;
                case SplitTunnelMode.Permit:
                    apps = GetApps(SplitTunnelingAllowApps);
                    break;
            }

            return apps;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private T Get<T>([CallerMemberName] string propertyName = null)
        {
            return _storage.Get<T>(propertyName);
        }

        private void Set<T>(T value, [CallerMemberName] string propertyName = null)
        {
            var toType = UnwrapNullable(typeof(T));
            if (toType.IsValueType || toType == typeof(string))
            {
                var oldValue = _storage.Get<T>(propertyName);
                if (EqualityComparer<T>.Default.Equals(oldValue, value))
                    return;
            }

            _storage.Set(propertyName, value);
            OnPropertyChanged(propertyName);
        }

        private T GetPerUser<T>([CallerMemberName] string propertyName = null)
        {
            _accessedPerUserProperties.Add(propertyName);

            return _userSettings.Get<T>(propertyName);
        }

        private void SetPerUser<T>(T value, [CallerMemberName] string propertyName = null)
        {
            _accessedPerUserProperties.Add(propertyName);

            var toType = UnwrapNullable(typeof(T));
            if (toType.IsValueType || toType == typeof(string))
            {
                var oldValue = _userSettings.Get<T>(propertyName);
                if (EqualityComparer<T>.Default.Equals(oldValue, value))
                    return;
            }

            _userSettings.Set(propertyName, value);
            OnPropertyChanged(propertyName);
        }

        private Type UnwrapNullable(Type type)
        {
            if (IsNullableType(type))
                return Nullable.GetUnderlyingType(type);

            return type;
        }

        private bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private void OnPropertiesChanged(IEnumerable<string> properties)
        {
            foreach (var property in properties)
            {
                OnPropertyChanged(property);
            }
        }

        private string[] GetApps(SplitTunnelingApp[] apps)
        {
            if (apps == null)
                return new string[0];

            return apps.Where(a => a.Enabled).Select(a => a.Path)
                .Union(apps.Where(a => a.Enabled)
                    .SelectMany(a => a.AdditionalPaths ?? new string[0]))
                .ToArray();
        }
    }
}
