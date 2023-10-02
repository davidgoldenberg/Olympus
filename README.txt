Usage: docker run -v C:\junk:/log <generated image> // e.g. docker run -v C:\junk:/log a706fb637546

When testing the application locally, files will be written to the root directory, e.g. C:\log.  Using the above command
will output the file to C:\junk.


Technical considerations/discussion points for the task:
1. Opening and closing a file among multiple threads is not an efficient way to synchronize writing to a single file.
  Since the limiter is ultimately synchronous access to a file, all of the threads behave as if they are synchronous no
  matter how many threads are spawned. They're all waiting for release.
2. An AutoResetEvent was used to synchronize access to the file without using waits as this class was designed specifically
  for this kind of use case.
3. Recent versions of .Net use Tasks instead of Threads for asynchronous processing.  For example if an asynchronous 
  file stream was used and the buffer was flushed after every write by 10 tasks, the processing time for the entire 100 rows
  would be milliseconds instead of seconds.  Passing around a singleton file stream among all tasks would reduce the number
  of file handles open and reduce the chances of collision for access.
4. Regarding exception and error handling.  There are many questions that would need to be asked if this were a real feature
  such as if a thread fails, should the rest of the threads continue on and have fewer lines in the log? Should the entire
  application shut down if there is one failure?  Should threads restart or have a logarithmic back off to reattempt before
  failing?
