<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="text" indent ="no" omit-xml-declaration="yes"/>

  <xsl:template match="/">
    <xsl:apply-templates select="ArrayOfExportedRKO/ExportedRKO" />
КонецФайла
  </xsl:template>

  <xsl:template match="ExportedRKO">
Новый документ РКО
Док(Внутр)     {"#",<xsl:value-of select="rkoId" />}
Дата           <xsl:value-of select="concat(msxsl:format-date(DocDate,'dd.MM.yyyy'),' ',msxsl:format-time(DocDate,'HH:mm:ss'))" />
Номер          <xsl:value-of select="DocNum" />
Время маршрута <xsl:value-of select="concat(msxsl:format-date(TripDateTime,'dd.MM.yyyy'),' ',msxsl:format-time(TripDateTime,'HH:mm:ss'))" />
ФизЛицо        <xsl:value-of select="DriverFIO" />
ФизЛицо(внутр) {"#",<xsl:value-of select="DriverId" />}
Доп. маршрут
Кассир         <xsl:value-of select="OperatorFIO" />
Основание      Договор
Осн. маршрут   <xsl:value-of select="TripName" />
Сумма          <xsl:value-of select="Sum" />
ТС             <xsl:value-of select="BusName" />
Приложение     <xsl:value-of select="Attachment" />
ПоДокументу    <xsl:value-of select="DriverPassport" />
Контрагент     <xsl:value-of select="OwnerName" />
КонтрагентВн   {"#",<xsl:value-of select="OwnerId" />} 
Ответственный  <xsl:value-of select="OperatorFIO" />
----------------------------</xsl:template>  
</xsl:stylesheet>