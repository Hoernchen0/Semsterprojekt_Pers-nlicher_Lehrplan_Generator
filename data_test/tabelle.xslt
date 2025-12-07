<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <html>
      <body>
        <table border="1">
          <tr>
            <th>Datum</th>
            <th>StartZeit</th>
            <th>EndZeit</th>
            <th>Thema</th>
            <th>Beschreibung</th>
            <th>Materialquelle</th>
            <th>Lernmethode</th>
            <th>Lernteam</th>
            <th>TeamMitglieder</th>
            <th>Priorität</th>
          </tr>

          <xsl:for-each select="Lernplan/Einheit">
            <tr>
              <td><xsl:value-of select="Datum"/></td>
              <td><xsl:value-of select="StartZeit"/></td>
              <td><xsl:value-of select="EndZeit"/></td>
              <td><xsl:value-of select="Thema"/></td>
              <td><xsl:value-of select="Beschreibung"/></td>
              <td><xsl:value-of select="Materialquelle"/></td>
              <td><xsl:value-of select="Lernmethode"/></td>
              <td><xsl:value-of select="Lernteam"/></td>
              <td><xsl:value-of select="TeamMitglieder"/></td>
              <td><xsl:value-of select="Prioritaet"/></td>
            </tr>
          </xsl:for-each>

        </table>
      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>
