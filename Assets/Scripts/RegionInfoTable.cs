using System.Collections.Generic;

public class RegionInfoTable
{
	private List<RegionInfo> m_regionInfoList;

	public RegionInfoTable()
	{
		m_regionInfoList = new List<RegionInfo>();
		m_regionInfoList.Add(new RegionInfo(1, "JP", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(2, "US", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(3, "GB", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(4, "FR", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(5, "IT", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(6, "DE", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(7, "ES", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(8, "RU", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(9, "PT", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(10, "BR", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(11, "CA", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(12, "AU", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(13, "KR", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(14, "CN", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(15, "TW", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(16, "HK", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(17, "AT", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(18, "BE", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(19, "BG", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(20, "DK", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(21, "FI", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(22, "GR", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(23, "HU", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(24, "IS", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(25, "IE", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(26, "NL", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(27, "NO", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(28, "PL", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(29, "SE", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(30, "CH", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(31, "IN", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(32, "ID", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(33, "IL", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(34, "MY", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(35, "PH", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(36, "SG", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(37, "TH", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(38, "TR", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(39, "VN", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(40, "AR", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(41, "CL", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(42, "CO", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(43, "MX", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(44, "NZ", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(45, "EG", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(46, "AL", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(47, "DZ", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(48, "AO", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(49, "AI", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(50, "AG", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(51, "AM", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(52, "AW", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(53, "AZ", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(54, "BS", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(55, "BH", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(56, "BD", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(57, "BB", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(58, "BY", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(59, "BZ", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(60, "BJ", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(61, "BM", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(62, "BT", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(63, "BO", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(64, "BA", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(65, "BW", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(66, "BN", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(67, "BF", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(68, "KH", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(69, "CM", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(70, "CV", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(71, "KY", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(72, "TD", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(73, "CD", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(74, "CR", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(75, "CI", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(76, "HR", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(77, "CY", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(78, "CZ", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(79, "DM", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(80, "DO", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(81, "EC", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(82, "SV", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(83, "EE", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(84, "FJ", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(85, "GA", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(86, "GM", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(87, "GH", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(88, "GD", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(89, "GT", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(90, "GW", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(91, "GY", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(92, "HT", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(93, "HN", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(94, "IR", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(95, "JM", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(96, "JO", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(97, "KZ", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(98, "KE", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(99, "KW", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(100, "KG", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(101, "LA", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(102, "LV", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(103, "LB", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(104, "LR", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(105, "LT", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(106, "LU", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(107, "MO", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(108, "MK", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(109, "MG", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(110, "MW", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(111, "ML", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(112, "MT", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(113, "MR", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(114, "MU", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(115, "FM", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(116, "MD", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(117, "MN", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(118, "MS", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(119, "MA", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(120, "MZ", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(121, "MM", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(122, "NA", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(123, "NP", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(124, "AN", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(125, "NI", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(126, "NE", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(127, "NG", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(128, "OM", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(129, "PK", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(130, "PW", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(131, "PA", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(132, "PG", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(133, "PY", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(134, "PE", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(135, "QA", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(136, "RO", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(137, "RW", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(138, "KN", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(139, "LC", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(140, "VC", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(141, "SA", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(142, "SN", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(143, "RS", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(144, "SC", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(145, "SL", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(146, "SK", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(147, "SI", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(148, "SB", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(149, "ZA", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(150, "LK", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(151, "SR", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(152, "SZ", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(153, "ST", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(154, "TJ", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(155, "TZ", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(156, "TG", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(157, "TT", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(158, "TN", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(159, "TM", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(160, "TC", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(161, "UG", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(162, "UA", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(163, "AE", "NON", "NON"));
		m_regionInfoList.Add(new RegionInfo(164, "UY", "NON", "ESRB"));
		m_regionInfoList.Add(new RegionInfo(165, "UZ", "NON", "NON"));
	}

	public RegionInfo GetInfo(int index)
	{
		if (index >= m_regionInfoList.Count)
		{
			return null;
		}
		return m_regionInfoList[index];
	}

	public RegionInfo GetInfo(string countryCode)
	{
		for (int i = 0; i < m_regionInfoList.Count; i++)
		{
			if (countryCode == m_regionInfoList[i].CountryCode)
			{
				return m_regionInfoList[i];
			}
		}
		return null;
	}
}
