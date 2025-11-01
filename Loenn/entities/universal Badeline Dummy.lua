local badelineBoost = {}

badelineBoost.name = "Partline/UBadelineDummy"
badelineBoost.depth = -1000000
--badelineBoost.texture = "objects/badelineboost/idle00"
local variantOptions = { "blue", "orange", "yellow", "green", "gray", "red", "pink", "purple", "magenta"}
local Names = {"sadeline","cadeline","dadeline","radeline","ladeline","hadeline","nadeline", "badeline", "sixty"}
local Directories = {"sadeline","cadeline","dadeline","green","ladeline","hadeline","nadeline", "badeline", "sixty"}
local Colors = {"2F69CE", "E38444", "FFE800", "009E2A", "AAC1C1", "D30000", "FF7CA0", "9b41c1", "C43684"}
badelineBoost.placements = {
    name = "colorful Badeline Dummy",
    data = {
        variant = "blue", 
        left = false,
        floating = true,
        animation = "",
        flag = "",
        ifset = false,
        render = false,
        soul = false
    }
}
badelineBoost.fieldInformation = {
    variant = {
        options = variantOptions,
        editable = false
    }
}

function badelineBoost.scale(room, entity)
    return entity.left and -1 or 1, 1
end

function badelineBoost.texture(room, entity)
    local index;
    for i, name in ipairs(variantOptions) do
        if name == entity.variant then
            index = i  -- Store the index
            break  -- Exit the loop once found
        end
    end
    local sprite = entity.floating and "jumpSlow03" or "idle00"
    
    return "characters/" .. Directories[index] .. "/" .. sprite--, characters/play/bangs00
end

return badelineBoost