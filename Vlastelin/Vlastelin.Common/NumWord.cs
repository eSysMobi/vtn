using System;

/// <summary>
/// ����� ����������� ����� ��������.
/// 3 �������� - �����, ������� � ������ ��� �������������
/// ������ ����� ������ (������, ����� � �.�.)
/// --------------------------------------
/// ����� - ���� ������� (dnkvpb@nm.ru)
/// </summary>

public class NumByWords 
{
	public static string RurPhrase (decimal money)
	{
		if (money < 0) return "�������� �����";
		return CurPhrase(money,"�����","�����","������","�������","�������","������");
	}

	public static string UsdPhrase (decimal money)
	{
		return CurPhrase(money,"������ ���","������� ���","�������� ���","����","�����","������");
	}

	public static string NumPhrase (ulong Value, bool IsMale) 
	{
		if (Value==0UL) return "����";
		string[] Dek1={""," ��"," ��"," ���"," ������"," ����"," �����"," ����"," ������"," ������"," ������"," �����������"," ����������"," ����������"," ������������"," ����������"," �����������"," ����������"," ������������"," ������������"};
		string[] Dek2={"",""," ��������"," ��������"," �����"," ���������"," ����������"," ���������"," �����������"," ���������"};
		string[] Dek3={""," ���"," ������"," ������"," ���������"," �������"," ��������"," �������"," ���������"," ���������"};
		string[] Th={"",""," �����"," �������"," ��������"," ��������"," ����������"," ����������"};
		string str="";
		for (byte th=1; Value>0; th++) 
		{
			ushort gr=(ushort)(Value%1000); 
			Value=(Value-gr)/1000;
			if (gr>0) 
			{
				byte d3=(byte)((gr-gr%100)/100);
				byte d1=(byte)(gr%10);
				byte d2=(byte)((gr-d3*100-d1)/10);
				if (d2==1) d1+=(byte)10;
				bool ismale=(th>2)||((th==1)&&IsMale);
				str=Dek3[d3]+Dek2[d2]+Dek1[d1]+EndDek1(d1,ismale)+Th[th]+EndTh(th,d1)+str;
			};
		};
		str=str.Substring(1,1).ToUpper()+str.Substring(2);
		return str;
	}

#region Private members
	private static string CurPhrase (decimal money, 
		string word1,string word234,string wordmore,
		string sword1,string sword234,string swordmore)
	{ 
		money=decimal.Round(money,2);
		decimal decintpart=decimal.Truncate(money);
		ulong intpart=decimal.ToUInt64(decintpart);
		string str=NumPhrase(intpart,true)+" ";
		byte endpart=(byte)(intpart%100UL); 
		if (endpart>19) endpart=(byte)(endpart%10);
		switch (endpart) 
		{
			case 1: str+=word1; break;
			case 2:
			case 3:
			case 4: str+=word234; break;
			default: str+=wordmore; break;
		}
		byte fracpart=decimal.ToByte((money-decintpart)*100M);
		str+=" "+((fracpart<10)?"0":"")+fracpart.ToString()+" ";
		if (fracpart>19) fracpart=(byte)(fracpart%10);
		switch (fracpart) 
		{
			case 1: str+=sword1; break;
			case 2:
			case 3:
			case 4: str+=sword234; break;
			default: str+=swordmore; break;
		};
		return str;
	}
	private static string EndTh(byte ThNum, byte Dek)
	{
		bool In234=((Dek>=2)&&(Dek<=4));
		bool More4=((Dek>4)||(Dek==0));
		if (((ThNum>2)&&In234)||((ThNum==2)&&(Dek==1))) return "�";
		else if ((ThNum>2)&&More4) return "��";
		else if ((ThNum==2)&&In234) return "�";
		else return "";
	}
	private static string EndDek1(byte Dek, bool IsMale)
	{
		if ((Dek>2)||(Dek==0)) return "";
		else if (Dek==1) 
		{
			if (IsMale) return "��";
			else return "��";
		}
		else 
		{
			if (IsMale) return "�"; 
			else return "�";
		}
	}
#endregion
}
