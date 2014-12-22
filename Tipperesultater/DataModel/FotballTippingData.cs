﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Windows.Data.Json;

namespace Tipperesultater.Data
{
    class FotballTippingData : ResultatData
    {
        private Windows.Data.Json.JsonObject jsonObjectLotto;

        private static string GetFotballtippingResultater(JsonArray res2)
        {
            String text = null;
            int pteller = 1;

            foreach (JsonValue r22 in res2)
            {
                JsonArray j2 = r22.GetArray();
                text += String.Join("", j2.Select((x, i) => i == 0 ? x.GetString() : decode(x.GetNumber())));
                if (pteller % 3 == 0)
                {
                    text += "\r\n";
                }
                pteller++;



            }
            return text;
        }

        private static string decode(double number)
        {
            switch ((int)number)
            {
                case 0:
                    return " H\r\n";
                case 1:
                    return " U\r\n";
                case 2:
                    return " B\r\n";
                default:
                    return "\r\n";
            }
        }

        public FotballTippingData(Windows.Data.Json.JsonObject jsonObjectLotto, string spillnavn) : base(spillnavn)
        {
            this.jsonObjectLotto = jsonObjectLotto;


            System.Diagnostics.Debug.WriteLine("fotballtipping");
            string halvTidOverskrift = "HALFTIME";
            string helTidOverskrift = "FULLTIME";
            string antallVinnereOverskrift = "WINNERS";
            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("nb"))
            {
                halvTidOverskrift = "HALVTID";
                helTidOverskrift = "HELTID";
                antallVinnereOverskrift = "VINNERE";
            }
            var b = jsonObjectLotto["gameDays"].GetArray();
            foreach (JsonValue obj in b)
            {
                {
                    if (obj.ValueType != JsonValueType.Null)
                    {
                        JsonObject ob1 = obj.GetObject();

                        string dd1 = ob1["drawDate"].GetString();
                        System.Diagnostics.Debug.WriteLine(dd1);

                        DateTime trekningspunkt2 = DateTime.ParseExact(dd1, "yyyy,MM,dd,HH,mm,ss", CultureInfo.CurrentCulture);
                        string trekningspunktAsString2 = trekningspunkt2.ToString("dddd d. MMMM", CultureInfo.CurrentCulture);

                        JsonArray events = ob1["events"].GetArray();
                        StringBuilder kamper = new StringBuilder();
                        String kampstatus = "";
                        int tellert = 1;
                        int statusteller = 0;
                        StringBuilder liveResultat = new StringBuilder();
                        StringBuilder liveResultatStatus = new StringBuilder();
                        foreach (JsonValue ev in events)
                        {

                            JsonArray arra = ev.GetArray();
                            String lag1 = arra[1].GetString();
                            String lag2 = arra[2].GetString();
                            String kamp = lag1 + " - " + lag2;
                            if (kamp.Count() > 39) 
                            {
                                kamp = kamp.Substring(0, 39) + ".";
                            }
                            kamper.Append(String.Format("{0}\r\n", kamp));
                            liveResultat.Append(String.Format("{0}\r\n", arra[5].GetString()));
                            liveResultatStatus.Append(String.Format("({0})\r\n", arra[4].GetString()));
                            if (tellert % 3 == 0)
                            {
                                kamper.Append("\r\n");
                                liveResultat.Append("\r\n");
                                liveResultatStatus.Append("\r\n");
                            }
                            tellert++;
                            String status = arra[4].GetString();
                            if (status.Equals("Slutt"))
                            {
                                statusteller++;
                            }

                        }
                        if (statusteller == 12)
                        {
                            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("nb"))
                            {
                                kampstatus = "Alle kamper er ferdig";
                            }
                            else
                            {
                                kampstatus = "All matches are finished";
                            }
                        }
                        else
                        {
                            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("nb"))
                            {
                                kampstatus = "Ikke alle kamper er ferdig";
                            }
                            else
                            {
                                kampstatus = "Not all matches are finished";
                            }

                        }


                        StringBuilder halvtid = new StringBuilder();
                        StringBuilder heltid = new StringBuilder();
                        StringBuilder premieTekst = new StringBuilder(String.Format("{0}\r\n", halvTidOverskrift));
                        StringBuilder premieVerdi = new StringBuilder("\r\n");
                        StringBuilder antallVinnere = new StringBuilder(antallVinnereOverskrift); //initialiseres ikke med linjeskift fordi linjeskift fordi siste element i linja
                        int teller = 0;
                        JsonArray results = ob1["matchStages"].GetArray();
                        foreach (JsonValue ev in results)
                        {

                            String resultatText = null;
                            JsonObject r2 = ev.GetObject();
                            JsonArray res2 = r2["results"].GetArray();
                            resultatText = GetFotballtippingResultater(res2);

                            if (teller == 0)
                            {
                                halvtid.Append(resultatText);
                            }
                            else
                            {
                                heltid.Append(resultatText);
                            }

                            teller++;

                            res2 = r2["prizes"].GetArray();

                            IList abba = res2.Reverse().ToList();

                            foreach (JsonValue j2 in abba)
                            {

                                JsonArray j22 = j2.GetArray();
                                String text = String.Join("", j22.Select((x, i) => i == 0 ? x.GetString().Replace("av", CultureInfo.CurrentCulture.TwoLetterISOLanguageName.Equals("nb") ? "av" : "of") : i == 1 ? "" : "\r\n"));
                                premieTekst.Append(text);

                                String text2 = String.Join("", j22.Select((x, i) => i == 1 ?
                                        x.ValueType == JsonValueType.String ? x.GetString() :
                                        x.GetNumber().ToString("### ### ### kr") : i == 0 ? "" : "\r\n"));
                                premieVerdi.Append(text2);

                                String antallVinnerePrRette = String.Join("", j22.Select((x, i) => i == 2 ?
                                        x.ValueType == JsonValueType.String ? x.GetString() :
                                        x.GetNumber() == 0 ? "0" : x.GetNumber().ToString("### ###") : i == 1 ? "" : "\r\n"));
                                antallVinnere.Append(antallVinnerePrRette);
                            }
                            if (teller == 1)
                            {
                                premieTekst.Append(String.Format("\r\n{0}\r\n", helTidOverskrift));   
                                premieVerdi.Append("\r\n\r\n");
                                antallVinnere.Append("\r\n\r\n");
                            }

                        }
                        this.Heltid = heltid.ToString();
                        this.Kamper = kamper.ToString();
                        this.Halvtid = halvtid.ToString();
                        this.Trekningsdato = trekningspunktAsString2;
                        this.Premienavn = premieTekst.ToString();
                        this.Premietall = premieVerdi.ToString();
                        this.Kampstatus = kampstatus;
                        this.LiveResultat = liveResultat.ToString();
                        this.LiveResultatStatus = liveResultatStatus.ToString();
                        this.AntallVinnere = antallVinnere.ToString();
                    }
                }
            }


        }



        public string Heltid { get; protected set; }
        public string Kamper { get; protected set; }
        public string Halvtid { get; protected set; }
        public string Trekningsdato { get; protected set; }
        public string Premienavn { get; protected set; }
        public string Premietall { get; protected set; }
        public string Kampstatus { get; protected set; }
        public string LiveResultat { get; protected set; }
        public string LiveResultatStatus { get; protected set; }
        public string AntallVinnere { get; protected set; }
    }
}
