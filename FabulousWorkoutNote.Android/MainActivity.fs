// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace FabulousWorkoutNote.Android

open System

open Android.App
open Android.Content
open Android.Content.PM
open Android.Runtime
open Android.Views
open Android.Widget
open Android.OS
open Xamarin.Forms.Platform.Android
open FabulousWorkoutNote
open System.IO

[<Activity (Label = "FabulousWorkoutNote.Android", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = (ConfigChanges.ScreenSize ||| ConfigChanges.Orientation))>]
type MainActivity() =
    inherit FormsAppCompatActivity()

    let getDbPath() =
        let path = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        Path.Combine(path, "data.db3");

    override this.OnCreate(bundle: Bundle) =
        FormsAppCompatActivity.TabLayoutResource <- Resources.Layout.Tabbar
        FormsAppCompatActivity.ToolbarResource <- Resources.Layout.Toolbar

        base.OnCreate (bundle)
        Xamarin.Essentials.Platform.Init(this, bundle)
        Xamarin.Forms.Forms.Init(this, bundle)
        let dbPath = getDbPath()
        let app = FabulousWorkoutNote.App(dbPath)
        this.LoadApplication(app)

    override this.OnRequestPermissionsResult(requestCode: int, permissions: string[], [<GeneratedEnum>] grantResults: Android.Content.PM.Permission[]) =
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults)
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults)
