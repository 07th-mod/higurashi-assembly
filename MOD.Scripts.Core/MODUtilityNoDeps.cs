using System;
using System.IO;
using MOD.Scripts.Core.UnityLoggerShim;

/// <summary>
/// This file is used by both the actual main mod, and also the
/// "script compiler only" version of the mod we run in Github CI
///
/// It ensures the "script compiler only" can compile without using any Unity dependencies
/// </summary>
public static class MODUtilityNoDeps
{
	static public void TryDelete(string path)
	{
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}

	static public void MoveWithOverwrite(string src, string dst)
	{
		TryDelete(dst);
		File.Move(src, dst);
	}

	// Restores a backup (from WriteAllBytesSemiAtomic()) if 'path' doesn't exist
	// If you passed a custom backupExt to WriteAllBytesSemiAtomic(), then you must use the same
	// backupExt so the backup file can be found.
	//
	// Note: It would be possible to "complete" the transaction if we knew the temporary file was
	// fully written, but not yet moved, but the complexity/chance/benefit tradeoff is not worth it
	// for this application, as the files we are moving are very small.
	static public bool RestoreSemiAtomicBackupIfRequired(string path, string backupExt = ".temporarybackup")
	{
		string backupPath = path + backupExt;

		// If the target file already exists, take no action
		if (File.Exists(path))
		{
			return false;
		}

		// If there is no backup to restore, take no action
		if (!File.Exists(backupPath))
		{
			return false;
		}

		// If the target file doesn't exist, and there is a backup, restore the backup
		File.Move(backupPath, path);

		return true;
	}

	// Try to write to a new or existing file atomically, such that the final file is never half-written.
	// There is also File.Replace, but I'm not sure if it works the way I'd expect, so I have written my own function below.
	// I also avoid using Path.GetTempFileName as sometimes can have problems moving files if they are on different drives on Windows
	static public void WriteAllBytesSemiAtomic(string path, byte[] bytes, string backupExt = ".temporarybackup", string tempExt=".temporary", bool showToast = true)
	{
		string tempPath = path + tempExt;
		string backupPath = path + backupExt;

		bool backupWasCreated = false;
		bool finalMoveOK = false;
		try
		{
			// Try move existing file to backup path, if it exists.
			if(File.Exists(path))
			{
				MoveWithOverwrite(path, backupPath);
				backupWasCreated = true;
			}

			// Write to a temporary file
			File.WriteAllBytes(tempPath, bytes);

			// Move the temporary file to the final path, overwriting the original
			MoveWithOverwrite(tempPath, path);
			finalMoveOK = true;
		}
		catch(Exception e)
		{
			// Attempt to restore the backup as something went wrong
			// Only attempt to restore the backup if flag says backup was OK, in case there is some old backup file lying around
			try
			{
				if (!finalMoveOK && backupWasCreated && File.Exists(backupPath))
				{
					// If the backup file exists, restore it. If there is some problem just ignore it.
					MoveWithOverwrite(backupPath, path);
				}
			}
			catch(Exception restoreException)
			{
				Debug.Log($"Failed to restore backup - {restoreException}");
			}

			Debug.Log(e.ToString());
		}
		finally
		{
			// If something fails, ensure the temporary file is deleted
			TryDelete(tempPath);

			// Delete the backup file as the move was successful
			if (finalMoveOK)
			{
				TryDelete(backupPath);
			}
		}
	}
}
