# KaraokeShow

>This plugin is still under development. You can issue bugs or wishlist on this Github repo.

KaraokeShow is a MusicBee plugin which provided a LRC parsing framework to help building karaoke style lyrics display plugin on MusicBee. It can parse LRC file, calculate timespan and notify the display plugin to update lyric and percentage of sentences. Display plugin only needs to force on presentating lyrics.

KaraokeShow is a framework so that it does not contain any lyrics display function. However, we provide KaraokeShowMainPlugin, an internal plugin which can display lyrics in karaoke style on desktop. You can add more display plugin for KaraokeShow. KaraokeShow plugin is different from MusicBee plugin, so it usually can not be loaded directly by MusicBee.

KaraokeShow contains a lyrics scraping engine which is independent of LyricsRetrieval of MusicBee because this plugin starts developping at a very early time when dynamic LRC displaying seems having not supported fully by MusicBee. At this time, using MusicBee internal lyrics loader is a better choice, but you can also enable lyrics scraping engine of KaraokeShow in plugin settings. At the same time, KaraokeShow provides convertor which can import lyrics scrapers for KaraokeShow into MusicBee as LyricsRetrieval.

In general, KaraokeShow(with KaraokeShowMainPlugin) is a desktop lyrics display plugin for MusicBee.



## Feature

KaraokeShow can parse common LRC files. It can also parse compact LRC file, which more than one timestamp share the same lyric. But accurate LRC file, which each word is applied with a timestamp, has not been supported.

## Language

Lyrics parser supports every language. The following language support list is language support in setting panel.

* English
* Simplified Chinese
* Japanese (by M.C Wong)