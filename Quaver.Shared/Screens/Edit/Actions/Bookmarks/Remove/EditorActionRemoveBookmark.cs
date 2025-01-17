using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Quaver.API.Maps;
using Quaver.API.Maps.Structures;
using Quaver.Shared.Screens.Edit.Actions.Bookmarks.Add;

namespace Quaver.Shared.Screens.Edit.Actions.Bookmarks.Remove
{
    [MoonSharpUserData]
    public class EditorActionRemoveBookmark : IEditorAction
    {
        public EditorActionType Type { get; } = EditorActionType.RemoveBookmark;

        private EditorActionManager ActionManager { get; }

        private Qua WorkingMap { get; }

        public BookmarkInfo Bookmark { get; }

        [MoonSharpVisible(false)]
        public EditorActionRemoveBookmark(EditorActionManager manager, Qua map, BookmarkInfo bookmark)
        {
            ActionManager = manager;
            WorkingMap = map;
            Bookmark = bookmark;
        }

        [MoonSharpVisible(false)]
        public void Perform()
        {
            WorkingMap.Bookmarks.Remove(Bookmark);
            ActionManager.TriggerEvent(Type, new EditorActionBookmarkRemovedEventArgs(Bookmark));
        }

        [MoonSharpVisible(false)]
        public void Undo() => new EditorActionAddBookmark(ActionManager, WorkingMap, Bookmark).Perform();
    }
}
