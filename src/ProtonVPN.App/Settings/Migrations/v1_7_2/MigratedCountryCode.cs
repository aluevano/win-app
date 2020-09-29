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

using ProtonVPN.Core.Servers.Models;

namespace ProtonVPN.Settings.Migrations.v1_7_2
{
    internal class MigratedCountryCode
    {
        private readonly string _country;
        private readonly Server _server;

        public MigratedCountryCode(string country, Server server)
        {
            _country = country;
            _server = server;
        }

        public static implicit operator string(MigratedCountryCode item) => item.Value();

        public string Value()
        {
            if (!string.IsNullOrEmpty(_country))
                return _country.ToUpper();

            return _server?.ExitCountry?.ToUpper();
        }
    }
}